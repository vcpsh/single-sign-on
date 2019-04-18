using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models.Tribe
{
    public class TribeLr : LdapGroup
    {
        public TribeLr()
        {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeLr);
        }
        
        
        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeLr}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeLr));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;
    }
}
