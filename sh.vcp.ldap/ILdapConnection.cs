using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace sh.vcp.ldap
{
    public interface ILdapConnection
    {
        Task<TModel> SearchFirst<TModel>(string baseDn, string filter, string objectClass, int scope, string[] attributes,
            bool expectUnique, CancellationToken cancellationToken = default) where TModel : LdapModel, new();
        
        Task<ICollection<TModel>> Search<TModel>(string baseDn, string filter, string objectClass, int scope, string[] attributes,
            CancellationToken cancellationToken = default) where TModel : LdapModel, new();
        
        Task<TModel> Read<TModel>(string dn, CancellationToken cancellationToken = default) where TModel: LdapModel, new ();

        Task<bool> Bind(string dn, string password, CancellationToken cancellationToken = default);
    }
}