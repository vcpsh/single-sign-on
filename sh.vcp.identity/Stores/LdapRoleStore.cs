using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Stores
{
    public class LdapRoleStore : IRoleStore<LdapRole>
    {
        /// <inheritdoc cref="IRoleStore{TRole}" />
        public void Dispose() {
            // there are no resources to dispose
        }

        public Task<IdentityResult> CreateAsync(LdapRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(LdapRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(LdapRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(LdapRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(LdapRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(LdapRole role, string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(LdapRole role, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(LdapRole role, string normalizedName,
            CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<LdapRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<LdapRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
