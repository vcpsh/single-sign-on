using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using sh.vcp.identity.Extensions;
using sh.vcp.identity.Managers;
using sh.vcp.identity.Model;
using sh.vcp.ldap.Extensions;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace sh.vcp.sso.server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this._configuration = configuration;
            this._env = env;
            loggerFactory.AddConsole(this._configuration.GetSection("Logging"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            if (this._env.IsProduction())
            {
                services.Configure<MvcOptions>(options =>
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                });
            }

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddVcpShLdap(this._configuration);
            services.AddVcpShIdentity();

            // TODO: don't use the devloper signing credential and move resources and clients to database
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<LdapUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseMySql(this._configuration.GetConnectionString("IdentityConfig"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseMySql(this._configuration.GetConnectionString("IdentityOperational"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })
                .AddProfileService<ProfileManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseIdentityServer();
            app.UseStaticFiles();
            app.Run( async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(this._env.WebRootPath,"index.html"));
            });
            app.UseMvcWithDefaultRoute();
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configCtx = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configCtx.Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            }

            // Disabled because of an issue
//            if (env.IsProduction())
//            {
//                var options = new RewriteOptions()
//                .AddRedirectToHttps();
//
//                app.UseRewriter(options);
//            }
        }
    }
}