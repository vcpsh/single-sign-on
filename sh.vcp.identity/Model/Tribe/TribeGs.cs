using System.Collections.Generic;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;
using System.Linq;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeGs : LdapGroup
    {
        public TribeGs() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeGs);
        }
        
        
        protected new static readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeGs}).ToList();
        
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeGs));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeGs.Props;
    }
}