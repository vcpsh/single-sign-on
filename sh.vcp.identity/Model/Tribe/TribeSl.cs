using System.Collections.Generic;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeSl : LdapGroup
    {
        public TribeSl() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeSl);
        }

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeSl));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeSl.Props;
    }
}