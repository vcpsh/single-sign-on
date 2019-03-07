using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models.Tribe
{
    public class TribeGs : LdapGroup
    {
        public TribeGs() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeGs);
        }
        
        
        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeGs}).ToList();
        
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeGs));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;
    }
}
