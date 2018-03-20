using System;
using System.Threading;
using System.Threading.Tasks;
using sh.vcp.identity.Model;
using sh.vcp.ldap;

namespace sh.vcp.identity.Managers
{
    internal class LdapLoginManager: ILoginManager<LdapUser>
    {
        private readonly ILdapConnection _connection;

        /// <inheritdoc />
        public LdapLoginManager(ILdapConnection connection)
        {
            this._connection = connection;
        }

        public async Task<bool> Login(LdapUser user, string password, CancellationToken cancellationToken)
        {
            return await this._connection.Bind(user.Dn, password, cancellationToken);
        }

        public void Dispose()
        {
        }
    }
}