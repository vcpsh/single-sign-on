using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models
{
    public class Division : LdapGroup
    {
        public Division() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.Division);
        }

        private static readonly Dictionary<PropertyInfo, LdapAttr>
            Props = LdapAttrHelper.GetLdapAttrs(typeof(Division));

        public new static readonly string[] LoadProperties = new[] {
            LdapProperties.DepartmentId
        }.Concat(LdapGroup.LoadProperties).ToArray();

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Division.Props;

        [JsonProperty("DepartmentId")]
        [LdapAttr(LdapProperties.DepartmentId, typeof(int))]
        public int DepartmentId { get; set; }
    }
}