using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeSl : LdapGroup
    {
        public TribeSl() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeSl);
        }
        
        
        protected new static readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeSl}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeSl));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeSl.Props;
    }
}