using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models.Tribe
{
    public class TribeGroup: LdapGroup
    {
        public TribeGroup()
        {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeGroup);
        }

        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeGroup}).ToList();
        
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeGroup));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;
    }
}
