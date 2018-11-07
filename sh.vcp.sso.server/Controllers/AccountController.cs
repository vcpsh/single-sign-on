using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NETCore.MailKit.Core;
using sh.vcp.identity.Managers;
using sh.vcp.identity.Model;
using sh.vcp.identity.Stores;
using sh.vcp.sso.server.Models;
using sh.vcp.sso.server.Utilities;

namespace sh.vcp.sso.server.Controllers
{
    [Route("/api/account")]
    public class LoginController : Controller
    {
        private readonly IEventService _events;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILoginManager<LdapUser> _login;
        private readonly IEmailService _mail;
        private readonly SigningCredentials _signingCredentials;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ILdapUserStore<LdapUser> _users;
        private readonly IViewRenderService _viewRenderService;

        public LoginController(IIdentityServerInteractionService interaction, ILdapUserStore<LdapUser> users,
            IEventService events, ILoginManager<LdapUser> login, IEmailService mail,
            IViewRenderService viewRenderService, SigningCredentials signingCredentials) {
            this._interaction = interaction;
            this._users = users;
            this._events = events;
            this._login = login;
            this._mail = mail;
            this._viewRenderService = viewRenderService;
            this._signingCredentials = signingCredentials;
            this._tokenValidationParameters = new TokenValidationParameters {
                ValidIssuer = "https://account.vcp.sh",
                ValidateAudience = false,
                IssuerSigningKey = this._signingCredentials.Key,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateTokenReplay = true
            };
        }

        /// <summary>
        ///     Handles postback from cancel on the login page
        /// </summary>
        [HttpPost("cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel([FromBody] CancelViewModel vm, CancellationToken cancellationToken) {
            var ctx = await this._interaction.GetAuthorizationContextAsync(vm.ReturnUrl);
            if (ctx != null) {
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
        ///     Handle postback from username/password login
        /// </summary>
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken) {
            // something went wrong, show form with error
            if (!this.ModelState.IsValid) return this.BadRequest();

            // validate username/password against in-memory store
            var user = await this._users.FindByNameAsync(model.Username, cancellationToken);
            if (user != null && user.EmailVerified) {
                var validation = await this._login.Login(user, model.Password, cancellationToken);
                if (validation) {
                    await this._events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    var props = new AuthenticationProperties();
                    if (AccountOptions.AllowRememberLogin && model.Remember) {
                        props.IsPersistent = true;
                        props.ExpiresUtc = DateTime.UtcNow.Add(AccountOptions.RememberMeLoginDuration);
                    }

                    props.AllowRefresh = true;

                    // issue authentication cookie with subject ID and username
                    await this.HttpContext.SignInAsync(user.Id, user.UserName, props);

                    // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                    // the IsLocalUrl check is only necessary if you want to support additional local pages, otherwise IsValidReturnUrl is more strict
                    if (this._interaction.IsValidReturnUrl(model.ReturnUrl) || this.Url.IsLocalUrl(model.ReturnUrl))
                        return this.Ok(new {model.ReturnUrl});

                    return this.Ok(new {ReturnUrl = "/"});
                }
            }

            await this._events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));

            return this.BadRequest();
        }

        /// <summary>
        ///     Handle logout page postback
        /// </summary>
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout([FromBody] string logoutId) {
            string idp = null;
            if (this.User?.Identity.IsAuthenticated == true) {
                // delete local authentication cookie
                await this.HttpContext.SignOutAsync();

                // raise the logout event
                await this._events.RaiseAsync(new UserLogoutSuccessEvent(this.User.GetSubjectId(),
                    this.User.GetDisplayName()));

                idp = this.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp == IdentityServerConstants.LocalIdentityProvider) idp = null;
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (idp != null) return this.Ok(new {ReturnUrl = $"/logoutredirect?logoutId{logoutId}", Provider = idp});

            return this.Ok(new {ReturnUrl = "/"});
        }

        /// <summary>
        ///     Handle forgot page postback
        /// </summary>
        [HttpPost("forgot")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot([FromBody] ForgotViewModel vm, CancellationToken cancellationToken) {
            var user = await this._users.FindByEmailAsync(vm.Email, cancellationToken);
            if (user == null) return this.BadRequest();

            Claim[] claims = {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim("reset_password", "yes"),
                new Claim(JwtClaimTypes.Subject, user.Id)
            };

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                "https://account.vcp.sh",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                notBefore: DateTime.UtcNow,
                signingCredentials: this._signingCredentials
            ));

            var content = $@"
<h3>Account zurücksetzen</h3>
<p>Mit diesem <a href='https://account.vcp.sh/reset?token={token}'>Link</a> kannst du das Passwort für deinen VCP-SH-Account zurücksetzen.</p>
<p>Aus Sicherheitsgründen ist der Link nur eine Stunde gültig.
<br>
<p>Falls du keinen Reset angefordert hast kannst du diese Email ignorieren.</p>
<br>
<p>Bei weiteren Fragen kannst du dich an die <a href='mailto:lgs@vcp.sh'>Landesgeschäftsstelle</a> wenden.</p>
";
            await this._mail.SendAsync(user.Email, "Passwort vergessen", content, true);
            return this.Ok();
        }

        /// <summary>
        ///     Handle reset page postback
        /// </summary>
        [HttpPost("reset")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset([FromBody] ResetViewModel vm, CancellationToken cancellationToken) {
            SecurityToken token;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var claims =
                new JwtSecurityTokenHandler().ValidateToken(vm.JwtTokenString, this._tokenValidationParameters,
                    out token);

            if (!claims.HasClaim("reset_password", "yes") || vm.Password != vm.ConfirmPassword)
                return this.BadRequest();

            var user = await this._users.FindByIdAsync(claims.GetSubjectId(), cancellationToken);

            if (await this._users.SetUserPasswordAsync(user, vm.Password, cancellationToken)) return this.Ok();

            return this.BadRequest();
        }

        /// <summary>
        ///     Handle register page postback
        /// </summary>
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            Register([FromBody] RegisterViewModel vm, CancellationToken cancellationToken) {
            if (!this.ModelState.IsValid) return this.BadRequest();

            if (vm.Password != vm.ConfirmPassword) return this.BadRequest("Passwords do not match");

            var member = await this._users.FindByIdAsync(vm.Id, cancellationToken);
            if (member == null) return this.BadRequest("Unknown VCP-ID");

            if (member.UserName != null) return this.BadRequest("Account exists");

            var mByUsername = await this._users.FindByNameAsync(vm.Username, cancellationToken);
            if (mByUsername != null) return this.BadRequest("Username used by another account");

            var mByMail = await this._users.FindByEmailAsync(vm.Email, cancellationToken);
            if (mByMail != null) return this.BadRequest("Email used by another account");

            var res = await this._users.SetUserPasswordAsync(member, vm.Password, cancellationToken);
            if (!res) return this.ServerError(new Exception("Sett user password failed"));

            Claim[] claims = {
                new Claim(JwtRegisteredClaimNames.Email, vm.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, vm.Username),
                new Claim("register", "yes"),
                new Claim(JwtClaimTypes.Subject, member.Id)
            };

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                "https://account.vcp.sh",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                notBefore: DateTime.UtcNow,
                signingCredentials: this._signingCredentials
            ));

            var content = $@"
<h3>VCP-SH-Account erstellen</h3>
<p>Mit diesem <a href='https://account.vcp.sh/confirm?token={token}'>Link</a> kannst du die Registrierung für deinen VCP-SH-Account abschließen.</p>
<p>Aus Sicherheitsgründen ist der Link nur eine Stunde gültig.
<br>
<p>Falls du keine Registrierung durchgeführt hast kannst du diese Email ignorieren.</p>
<br>
<p>Bei weiteren Fragen kannst du dich an die <a href='mailto:lgs@vcp.sh'>Landesgeschäftsstelle</a> wenden.</p>
";
            await this._mail.SendAsync(vm.Email, "Registrierung abschließen", content, true);

            return this.Ok();
        }

        /// <summary>
        ///     Handle confirm page postback
        /// </summary>
        [HttpPost("confirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm([FromBody] ConfirmViewModel vm,
            CancellationToken cancellationToken) {
            SecurityToken token;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var claims =
                new JwtSecurityTokenHandler().ValidateToken(vm.JwtTokenString, this._tokenValidationParameters,
                    out token);

            if (!claims.HasClaim("register", "yes")) return this.BadRequest();

            var user = await this._users.FindByIdAsync(claims.GetSubjectId(), cancellationToken);
            user.Email = claims.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            user.EmailVerified = true;
            user.UserName = claims.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;

            if (await this._users.CreateAsync(user, cancellationToken) != IdentityResult.Success) {
                return this.ServerError(new Exception("Create user failed"));
            }

            return this.Ok();
        }

        public class CancelViewModel
        {
            [Required]
            public string ReturnUrl { get; set; }
        }

        public class ForgotViewModel
        {
            [Required]
            public string Email { get; set; }
        }

        public class ResetViewModel
        {
            [Required]
            [JsonProperty("token")]
            public string JwtTokenString { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string ConfirmPassword { get; set; }
        }

        public class RegisterViewModel
        {
            [Required]
            public string Id { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string ConfirmPassword { get; set; }
        }

        public class ConfirmViewModel
        {
            [Required]
            [JsonProperty("token")]
            public string JwtTokenString { get; set; }
        }
    }
}