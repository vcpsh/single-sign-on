using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Novell.Directory.Ldap;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeLr: LdapGroup
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeLr));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeLr.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.TribeLr;
    }
}