using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using sh.vcp.identity.Managers;
using sh.vcp.identity.Model;
using sh.vcp.identity.Stores;
using sh.vcp.ldap.ChangeTracking;

namespace sh.vcp.identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVcpShIdentity(this IServiceCollection services) {
            // add identity types
            services.AddIdentity<LdapUser, LdapRole>()
                .AddDefaultTokenProviders();

            // identity services
            services.AddTransient<IUserStore<LdapUser>, LdapUserStore>();
            services.AddTransient<IRoleStore<LdapRole>, LdapRoleStore>();
            services.AddTransient<ILdapUserStore<LdapUser>, LdapUserStore>();
            services.AddTransient<ILoginManager<LdapUser>, LdapLoginManager>();
            services.AddTransient<IProfileService, ProfileManager>();
        }
    }
}