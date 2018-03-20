using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using sh.vcp.identity.Model;

namespace sh.vcp.identity.Managers
{
    /// <summary>
    /// IProfileService to integrate with ASP.NET Identity.
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.IProfileService" />
    public class ProfileManager : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<LdapUser> _claimsFactory;
        private readonly UserManager<LdapUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        public ProfileManager(
            UserManager<LdapUser> userManager,
            IUserClaimsPrincipalFactory<LdapUser> claimsFactory
        )
        {
            this._userManager = userManager;
            this._claimsFactory = claimsFactory;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await this._userManager.FindByIdAsync(sub);
            var principal = await this._claimsFactory.CreateAsync(user);
            var customClaims = await this._userManager.GetClaimsAsync(user);
            var filteredIdentityResources =
                context.RequestedResources.IdentityResources.Where(res =>
                    context.Client.AllowedScopes.Contains(res.Name));
            context.IssuedClaims.AddRange(customClaims.Where(claim =>
                filteredIdentityResources.Any(res => res.UserClaims.Contains(claim.Type))));
            context.AddRequestedClaims(principal.Claims);
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the user's account has been deactivated since they logged in).
        /// (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await this._userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}