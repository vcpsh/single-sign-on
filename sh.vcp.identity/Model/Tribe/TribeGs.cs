using System.Collections.Generic;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeGs : LdapGroup
    {
        public TribeGs() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeGs);
        }

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeGs));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeGs.Props;
    }
}