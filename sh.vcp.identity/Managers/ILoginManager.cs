using System;
using System.Threading;
using System.Threading.Tasks;

namespace sh.vcp.identity.Managers
{
    public interface ILoginManager<TUser> : IDisposable where TUser : class
    {
        Task<bool> Login(TUser user, string password, CancellationToken cancellationToken = default);
    }
}