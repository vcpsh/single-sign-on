using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace sh.vcp.ldap
{
    public interface ILdapModelWithChildren
    {
        /// <summary>
        /// Loads all the children of the <see cref="LdapModel"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task LoadChildren(ILdapConnection connection, CancellationToken cancellationToken);
        
        /// <summary>
        /// Returns all the children of the <see cref="LdapModel"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<LdapModel> GetChildren();
    }
}