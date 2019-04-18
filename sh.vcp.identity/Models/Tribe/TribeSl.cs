using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models.Tribe
{
    public class TribeSl : LdapGroup
    {
        public TribeSl()
        {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeSl);
        }
        
        
        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeSl}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeSl));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;
    }
}
