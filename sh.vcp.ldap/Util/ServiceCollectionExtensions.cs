using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace sh.vcp.ldap.Util
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVcpShLdap(this IServiceCollection services, IConfiguration config) {
            var ldapConfig = new LdapConfig();
            config.GetSection("ldap").Bind(ldapConfig);
            ldapConfig.AuthorizationConfigurationSection = config.GetSection("Authorization");
            services.AddSingleton(ldapConfig);
            services.AddTransient<ILdapConnection, LdapConnection>();
        }
    }
}