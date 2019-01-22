using System.Collections.Generic;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;
using System.Linq;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeGroup: LdapGroup
    {
        public TribeGroup() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeGroup);
        }

        protected new static readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeGroup}).ToList();
        
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeGroup));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeGroup.Props;
    }
}