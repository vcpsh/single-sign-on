using System.Collections.Generic;
using System.Reflection;
using sh.vcp.identity.Models;
using sh.vcp.ldap;
using System.Linq;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeLr : LdapGroup
    {
        public TribeLr() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.TribeLr);
        }
        
        
        protected new static readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.TribeLr}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(TribeLr));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => TribeLr.Props;
    }
}