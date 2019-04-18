using System;
using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Managers
{
    /// <inheritdoc />
    public class ProfileManager : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<LdapUser> _claimsFactory;
        private readonly UserManager<LdapUser> _userManager;
        private readonly ILogger<ProfileManager> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProfileService{TUser}" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        /// <param name="logger">The logger</param>
        public ProfileManager(
            UserManager<LdapUser> userManager,
            IUserClaimsPrincipalFactory<LdapUser> claimsFactory,
            ILogger<ProfileManager> logger
        ) {
            this._userManager = userManager;
            this._claimsFactory = claimsFactory;
            this._logger = logger;
        }

        /// <summary>
        ///     This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo
        ///     endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context) {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await this._userManager.FindByIdAsync(sub);
            if (user == null) {
                this._logger?.LogWarning("No user found matching subject Id: {0}", sub);
                return;
            }

            var claimsPrincipal = await this._claimsFactory.CreateAsync(user);
            if (claimsPrincipal == null) throw new Exception("ClaimsFactory failed to create a principal");
            context.AddRequestedClaims(claimsPrincipal.Claims);
        }

        /// <summary>
        ///     This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the
        ///     user's account has been deactivated since they logged in).
        ///     (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context) {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No subject Id claim present");

            var user = await this._userManager.FindByIdAsync(sub);
            if (user == null) {
                this._logger?.LogWarning("No user found matching subject Id: {0}", sub);
            }

            context.IsActive = user != null;
        }
    }
}
