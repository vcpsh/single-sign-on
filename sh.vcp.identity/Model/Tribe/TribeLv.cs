using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeLv : LdapGroup
    {
        public TribeLv() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeLv);
        }
        
        
        protected new static readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeLv}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeLv));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeLv.Props;
    }
}