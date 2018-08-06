using System.Collections.Generic;
using System.Reflection;
using Novell.Directory.Ldap;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeGs: LdapGroup
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeGs));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeGs.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.TribeGs;
    }
}