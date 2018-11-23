using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using sh.vcp.identity.Extensions;
using sh.vcp.identity.Managers;
using sh.vcp.identity.Model;
using sh.vcp.ldap.ChangeTracking;
using sh.vcp.ldap.Extensions;
using sh.vcp.sso.server.Utilities;

namespace sh.vcp.sso.server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            this._configuration = configuration;
            this._env = env;
            loggerFactory.AddConsole(this._configuration.GetSection("Logging"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            if (!this._env.IsDevelopment()) {
                services.AddAntiforgery(options => { options.HeaderName = "X-XSRF-TOKEN"; });
            }

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            // configure proxy stuff
            if (this._configuration.GetValue("Proxy", false)) {
                services.Configure<ForwardedHeadersOptions>(options => {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    options.RequireHeaderSymmetry = false;
                });
            }

            // configure smtp
            services.AddMailKit(optionsBuilder => {
                var options = new MailKitOptions();
                this._configuration.GetSection("Mail").Bind(options);
                optionsBuilder.UseMailKit(options);
            });

            // configure render service for html mails
            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.Configure<CookiePolicyOptions>(options => {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddCors();

            services.AddMvc(options => {
                    if (!this._env.IsDevelopment()) {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddVcpShLdap(this._configuration,
                builder => builder.UseMySql(this._configuration.GetConnectionString("ChangeTracking"),
                    sql => sql.MigrationsAssembly(migrationsAssembly)));
            services.AddVcpShIdentity();

            var identityServerBuilder = services.AddIdentityServer(o => {
                    o.UserInteraction.LoginUrl = "/login";
                    o.UserInteraction.LogoutUrl = "/logout";
                    o.IssuerUri = this._configuration.GetValue<string>("IssuerUrl");
                    o.PublicOrigin = o.IssuerUri;
                })
                .AddAspNetIdentity<LdapUser>()
                .AddConfigurationStore(options => {
                    options.ConfigureDbContext = builder => {
                        builder.UseMySql(this._configuration.GetConnectionString("IdentityConfig"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddOperationalStore(options => {
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600;
                    options.ConfigureDbContext = builder => {
                        builder.UseMySql(this._configuration.GetConnectionString("IdentityOperational"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddProfileService<ProfileManager>();

            // configure jwt secret
            services.AddSingleton(
                new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration.GetValue<string>("JwtSecret"))),
                    SecurityAlgorithms.HmacSha256));

            // configure identity server signing credential
            if (this._configuration.GetValue<string>("SigningCredential").IsNullOrEmpty()) {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
            else {
                var cert = new X509Certificate2(
                    this._configuration.GetValue<string>("SigningCredential"),
                    this._configuration.GetValue<string>("SigningCredentialPassword"));
                identityServerBuilder.AddSigningCredential(cert);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseHsts();
            }

            if (this._configuration.GetValue("Proxy", false)) {
                app.UseForwardedHeaders();
            }

            app.UseIdentityServer();
            app.UseCors();
            app.UseMvc();

            app.Use(async (ctx, next) => {
                await next();
                var path = ctx.Request.Path;
                if (!this._env.IsDevelopment() && (
                        string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase) ||
                        ctx.Response.StatusCode == StatusCodes.Status404NotFound)) {
                    var antiforgery = app.ApplicationServices.GetService<IAntiforgery>();
                    var tokens = antiforgery.GetAndStoreTokens(ctx);
                    ctx.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                        new CookieOptions() {HttpOnly = false});
                }

                if (string.Equals(ctx.Request.Path, "/", StringComparison.OrdinalIgnoreCase) ||
                    ctx.Response.StatusCode == 404) {
                    ctx.Response.StatusCode = 200;
                    ctx.Response.ContentType = "text/html";
                    await ctx.Response.SendFileAsync(Path.Combine(this._env.WebRootPath, "index.html"));
                }
            });
            if (bool.TryParse(this._configuration["SpaProxy"], out var spaProxy) && spaProxy) {
                app.UseSpa(spa => { spa.UseProxyToSpaDevelopmentServer(this._configuration["SpaProxyPath"]); });
            }
            else {
                app.UseStaticFiles();
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope()) {
                var configCtx = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configCtx.Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                
                var changeCtx = serviceScope.ServiceProvider.GetRequiredService<ChangeTrackingDbContext>();
                changeCtx.Database.Migrate();
            }
        }
    }
}