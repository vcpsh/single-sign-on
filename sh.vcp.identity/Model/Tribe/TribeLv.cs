using System.Collections.Generic;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeLv : LdapGroup
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeLv));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeLv.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.TribeLv;
    }
}