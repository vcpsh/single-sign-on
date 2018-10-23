using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace sh.vcp.ldap
{
    public interface ILdapConnection
    {
        Task<TModel> SearchFirst<TModel>(string baseDn, string filter, string objectClass, int scope,
            string[] attributes,
            bool expectUnique, CancellationToken cancellationToken = default) where TModel : LdapModel, new();

        Task<ICollection<TModel>> Search<TModel>(string baseDn, string filter, string objectClass, int scope,
            string[] attributes,
            CancellationToken cancellationToken = default) where TModel : LdapModel, new();

        Task<TModel> Read<TModel>(string dn, CancellationToken cancellationToken = default)
            where TModel : LdapModel, new();

        Task<bool> Bind(string dn, string password, CancellationToken cancellationToken = default);

        Task<TModel> Add<TModel>(TModel model, CancellationToken cancellationToken = default) where TModel : LdapModel;

        Task<TModel> AddChildren<TModel>(TModel model, CancellationToken cancellationToken = default)
            where TModel : LdapModel, ILdapModelWithChildren;

        Task<bool> Update<TModel>(TModel model, CancellationToken cancellationToken = default)
            where TModel : LdapModel, new();

        Task<bool> Update<TModel>(string dn, LdapModification[] ldapModifications,
            CancellationToken cancellationToken = default) where TModel : LdapModel, new();
    }
}