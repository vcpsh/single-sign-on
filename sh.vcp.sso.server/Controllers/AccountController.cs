using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using sh.vcp.identity.Managers;
using sh.vcp.identity.Model;
using Microsoft.AspNetCore.Http;
using sh.vcp.sso.server.Models;

namespace sh.vcp.sso.server.Controllers
{
    [Route("/api/account")]
    public class LoginController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IUserStore<LdapUser> _users;
        private readonly IEventService _events;
        private readonly ILoginManager<LdapUser> _login;

        public LoginController(IIdentityServerInteractionService interaction, IUserStore<LdapUser> users, IEventService events, ILoginManager<LdapUser> login)
        {
            this._interaction = interaction;
            this._users = users;
            this._events = events;
            this._login = login;
        }

        /// <summary>
        /// Handles postback from cancel on the login page
        /// </summary>
        [HttpPost("cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel([FromBody] CancelViewModel vm, CancellationToken cancellationToken)
        {
            var ctx = await this._interaction.GetAuthorizationContextAsync(vm.ReturnUrl);
            if (ctx != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await this._interaction.GrantConsentAsync(ctx, ConsentResponse.Denied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return this.Ok();
            }
            
            // since we don't have a valid context, then we just go back to the home page
            return this.BadRequest();
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken)
        {
            // something went wrong, show form with error
            if (!this.ModelState.IsValid) return this.BadRequest();
            
            // validate username/password against in-memory store
            var user = await this._users.FindByNameAsync(model.Username, cancellationToken);
            if (user != null)
            {
                    var validation = await this._login.Login(user, model.Password, cancellationToken);
                    if (validation)
                    {
                        await this._events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                        // only set explicit expiration here if user chooses "remember me". 
                        // otherwise we rely upon expiration configured in cookie middleware.
                        var props = new AuthenticationProperties();
                        if (AccountOptions.AllowRememberLogin && model.Remember)
                        {
                            props.IsPersistent = true;
                            props.ExpiresUtc = DateTime.UtcNow.Add(AccountOptions.RememberMeLoginDuration);
                        }

                        props.AllowRefresh = true;

                        // issue authentication cookie with subject ID and username
                        await this.HttpContext.SignInAsync(user.Id, user.UserName, props);

                        // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                        // the IsLocalUrl check is only necessary if you want to support additional local pages, otherwise IsValidReturnUrl is more strict
                        if (this._interaction.IsValidReturnUrl(model.ReturnUrl) || this.Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return this.Ok(new { model.ReturnUrl });
                        }

                        return this.Ok(new { ReturnUrl = "/" });
                    }
            }

            await this._events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));

            return this.BadRequest();
        }
        
        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout([FromBody] string logoutId)
        {
            string idp = null;   
            if (this.User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await this.HttpContext.SignOutAsync();

                // raise the logout event
                await this._events.RaiseAsync(new UserLogoutSuccessEvent(this.User.GetSubjectId(), this.User.GetDisplayName()));
                
                idp = this.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp == IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    idp = null;
                }
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (idp != null)
            {
                // this triggers a redirect to the external provider for sign-out
                return this.Ok(new { ReturnUrl = $"/logoutredirect?logoutId{logoutId}", Provider = idp });
            }

            return this.Ok(new { ReturnUrl = "/"});
        }

        public class CancelViewModel
        {
            [Required]
            public string ReturnUrl { get; set; }
        }
    }
}