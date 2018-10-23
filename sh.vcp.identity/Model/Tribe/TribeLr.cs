using System.Collections.Generic;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeLr : LdapGroup
    {
        public TribeLr() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeLr);
        }

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeLr));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeLr.Props;
    }
}