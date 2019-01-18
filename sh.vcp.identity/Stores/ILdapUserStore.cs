using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace sh.vcp.identity.Stores
{
    public interface ILdapUserStore<TUser> : IUserEmailStore<TUser>, IUserClaimStore<TUser> where TUser : class
    {
        /// <summary>
        ///     Sets the new userPassword
        /// </summary>
        Task<bool> SetUserPasswordAsync(TUser user, string password, CancellationToken cancellationToken = default);

        [Obsolete("Use CreateAsync with changedBy", true)]
        new Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default);
        
        Task<IdentityResult> CreateAsync(TUser user, string changedBy, CancellationToken cancellationToken = default);
    }
}