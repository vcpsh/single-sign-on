using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models.Tribe
{
    public class TribeLv : LdapGroup
    {
        public TribeLv() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeLv);
        }
        
        
        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeLv}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeLv));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;
    }
}
