using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sh.vcp.ldap.ChangeTracking;

namespace sh.vcp.ldap.Util
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVcpShLdap(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> dbContextOptionsBuilder) {
            var ldapConfig = new LdapConfig();
            config.GetSection("ldap").Bind(ldapConfig);
            ldapConfig.AuthorizationConfigurationSection = config.GetSection("Authorization");
            services.AddSingleton(ldapConfig);
            services.AddTransient<ILdapConnection, LdapConnection>();
            
            // add change tracking context
            if (ldapConfig.LogChanges) {
                services.AddDbContext<ChangeTrackingDbContext>(dbContextOptionsBuilder);
            }
            else {
                services.AddSingleton<ChangeTrackingDbContext>((ChangeTrackingDbContext) null);
            }
        }
    }
}