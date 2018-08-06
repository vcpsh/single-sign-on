using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using sh.vcp.identity.Claims;
using sh.vcp.identity.Extensions;
using sh.vcp.identity.Model;
using sh.vcp.identity.Model.Tribe;
using sh.vcp.identity.Models;
using sh.vcp.ldap;
using sh.vcp.ldap.Exceptions;
using ILdapConnection = sh.vcp.ldap.ILdapConnection;

namespace sh.vcp.identity.Stores
{
    /// <summary>
    /// A user store using the vcpsh ldap backend.
    /// </summary>
    internal class LdapUserStore : ILdapUserStore<LdapUser>
    {
        private readonly ILdapConnection _connection;
        private readonly LdapConfig _config;
        private readonly ILogger<LdapUserStore> _logger;

        public LdapUserStore(ILdapConnection connection, LdapConfig config, ILogger<LdapUserStore> logger)
        {
            this._connection = connection;
            this._config = config;
            this._logger = logger;
        }

        public async Task<LdapUser> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                return await this._connection.SearchFirst<LdapUser>(this._config.MemberDn,
                    $"{LdapProperties.Email}={email}",
                    LdapProperties.Member,
                    LdapConnection.SCOPE_SUB,
                    LdapUser.LoadProperties,
                    true,
                    cancellationToken);
            }
            catch (LdapSearchNotUniqueException)
            {
                // If we found multiple users matching the filter we return null for safety
                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, IdentityErrorCodes.UserStoreFindByEmail);
            }

            return null;
        }

        public async Task<bool> SetUserPasswordAsync(LdapUser user, string password,
            CancellationToken cancellationToken)
        {
            try
            {
                return await this._connection.Mod(user.Dn,
                    new[]
                    {
                        new LdapModification(LdapModification.REPLACE,
                            new LdapAttribute(LdapProperties.UserPassword, password))
                    }, cancellationToken);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, IdentityErrorCodes.SetUserPasswordAsync);
                return false;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Currently nothing has to be disposed, so this mehod is empty.
        }

        public Task<string> GetUserIdAsync(LdapUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(LdapUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(LdapUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(LdapUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToUpper());
        }

        public Task SetNormalizedUserNameAsync(LdapUser user, string normalizedName,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(LdapUser user, CancellationToken cancellationToken)
        {
            var res = await this._connection.Mod(user, cancellationToken);
            return res ? IdentityResult.Success : IdentityResult.Failed();
        }

        public Task<IdentityResult> UpdateAsync(LdapUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(LdapUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<LdapUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                return await this._connection.SearchFirst<LdapUser>(this._config.MemberDn,
                    $"{LdapProperties.CommonName}={userId}",
                    LdapProperties.Member,
                    LdapConnection.SCOPE_SUB,
                    LdapUser.LoadProperties,
                    true,
                    cancellationToken);
            }
            catch (LdapSearchNotUniqueException)
            {
                // If we found multiple users matching the filter we return null for safety
                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, IdentityErrorCodes.UserStoreFindById);
            }

            return null;
        }

        public async Task<LdapUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            try
            {
                return await this._connection.SearchFirst<LdapUser>(this._config.MemberDn,
                    $"{LdapProperties.Uid}={normalizedUserName}",
                    LdapProperties.Member,
                    LdapConnection.SCOPE_SUB,
                    LdapUser.LoadProperties,
                    true,
                    cancellationToken);
            }
            catch (LdapSearchNotUniqueException)
            {
                // If we found multiple users matching the filter we return null for safety
                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, IdentityErrorCodes.UserStoreFindByName);
            }

            return null;
        }

        public async Task<IList<Claim>> GetClaimsAsync(LdapUser user, CancellationToken cancellationToken)
        {
            try
            {
                List<Claim> claims = new List<Claim>();
                List<Claim> additionalClaims = new List<Claim>();

                // load divisions
                claims.AddRange((await this._connection.Search<Division>(this._config.GroupDn,
                        $"{LdapProperties.Member}={user.Id}",
                        LdapObjectTypes.Division, LdapConnection.SCOPE_SUB, Division.LoadProperties, cancellationToken))
                    .Select(div => new DivisionClaim(div)));

                // load division lgs
                List<string> divisionLgsGroups = this._config.AuthorizationConfigurationSection.GetChildren().Select(
                    child =>
                    {
                        var subDn = child.GetSection("lgs").GetChildren().Select(section => section.Value).First();

                        return $"{subDn},{this._config.GroupDn}";
                    }).ToList();
                await divisionLgsGroups.ForEachAsync(async dn =>
                {
                    var group = await this._connection.Read<VotedLdapGroup>(dn, cancellationToken);
                    if (group.MemberIds.Contains(user.Id))
                    {
                        var divisionId = dn.Replace($",{this._config.GroupDn}", "");
                        divisionId = divisionId.Substring(divisionId.LastIndexOf(",")).Replace(",cn=", "");
                        claims.Add(new IsDivisionLgsClaim(divisionId));
                    }
                });

                // load tribes
                claims.AddRange((await this._connection.Search<Tribe>(this._config.GroupDn,
                        $"{LdapProperties.Member}={user.Id}",
                        LdapObjectTypes.Tribe, LdapConnection.SCOPE_SUB, Tribe.LoadProperties, cancellationToken))
                    .Select(div => new TribeClaim(div)));

                // load tribe admin
                claims.ForEach(claim =>
                {
                    if (!(claim is TribeClaim tclaim)) return;
                    if (tclaim.Tribe.Gs.MemberIds.Contains(user.Id))
                    {
                        additionalClaims.Add(new IsTribeGsClaim(tclaim.Tribe));
                    }

                    if (tclaim.Tribe.Sl.MemberIds.Contains(user.Id))
                    {
                        additionalClaims.Add(new IsTribeSlClaim(tclaim.Tribe));
                    }
                });


                return claims.Concat(additionalClaims).ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, IdentityErrorCodes.UserStoreGetClaims);
                return null;
            }
        }

        public Task AddClaimsAsync(LdapUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceClaimAsync(LdapUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(LdapUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<LdapUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}