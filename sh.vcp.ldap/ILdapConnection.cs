using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace sh.vcp.ldap
{
    public interface ILdapConnection
    {
        [Obsolete("This is unsafe use better alternative")]
        Task<TModel> SearchFirst<TModel>(string baseDn, string filter, string objectClass, int scope,
            string[] attributes,
            bool expectUnique, CancellationToken cancellationToken = default) where TModel : LdapModel, new();

        Task<ICollection<TModel>> Search<TModel>(string baseDn, string filter, string objectClass, int scope, CancellationToken cancellationToken)
            where TModel : LdapModel, new();
        
        Task<TModel> Read<TModel>(string dn, CancellationToken cancellationToken)
            where TModel : LdapModel, new();

        Task<bool> Bind(string dn, string password, CancellationToken cancellationToken = default);

        Task<TModel> Add<TModel>(TModel model, string changedBy, CancellationToken cancellationToken = default) where TModel : LdapModel;

        Task<TModel> AddChildren<TModel>(TModel model, string changedBy, CancellationToken cancellationToken = default)
            where TModel : LdapModel, ILdapModelWithChildren;

        Task<bool> Update<TModel>(TModel model, string changedBy, CancellationToken cancellationToken = default)
            where TModel : LdapModel, new();

        Task<bool> Update<TModel>(string dn, LdapModification[] ldapModifications, string changedBy,
            CancellationToken cancellationToken = default) where TModel : LdapModel, new();
    }
}
