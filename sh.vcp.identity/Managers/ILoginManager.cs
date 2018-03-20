using System;
using System.Threading;
using System.Threading.Tasks;
using sh.vcp.identity.Model;

namespace sh.vcp.identity.Managers
{
    public interface ILoginManager<TUser>: IDisposable where TUser: class
    {
        Task<bool> Login(LdapUser user, string password, CancellationToken cancellationToken = default);
    }
}