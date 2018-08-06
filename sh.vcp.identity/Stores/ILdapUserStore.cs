using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace sh.vcp.identity.Stores
{
    public interface ILdapUserStore<TUser>: IUserClaimStore<TUser> where TUser : class
    {
        
        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="email" />.
        /// </summary>
        /// <param name="email">The user email to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="email" /> if it exists.
        /// </returns>
        Task<TUser> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the new userPassword
        /// </summary>
        Task<bool> SetUserPasswordAsync(TUser user, string password, CancellationToken cancellationToken = default);
    }
}