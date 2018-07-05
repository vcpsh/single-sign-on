using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace sh.vcp.ldap
{
    public interface ILdapModelWithChildren
    {
        Task LoadChildren(ILdapConnection connection, CancellationToken cancellationToken = default);
        ICollection<LdapModel> GetChildren();
    }
}