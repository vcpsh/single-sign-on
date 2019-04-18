using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sh.vcp.ldap.Caching;
using sh.vcp.ldap.ChangeTracking;

namespace sh.vcp.ldap.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVcpShLdap(this IServiceCollection services, IConfiguration config,
            Action<DbContextOptionsBuilder> dbContextOptionsBuilder) {
            var ldapConfig = new LdapConfig();
            config.GetSection("ldap").Bind(ldapConfig);
            ldapConfig.AuthorizationConfigurationSection = config.GetSection("Authorization");
            services.AddSingleton(ldapConfig);
            services.AddScoped<ILdapConnection, LdapConnection>();

            // add change tracking context
            if (ldapConfig.LogChanges) {
                services.AddDbContext<ChangeTrackingDbContext>(dbContextOptionsBuilder);
            }
            else {
                services.AddSingleton(_ => (ChangeTrackingDbContext) null);
            }

            // add cache
            if (ldapConfig.UseCache) {
                services.AddSingleton<ILdapCache>(new LdapCache());
            }
            else {
                services.AddSingleton(_ => (ILdapCache) null);
            }
        }
    }
}
