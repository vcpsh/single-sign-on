using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Novell.Directory.Ldap;
using sh.vcp.identity.Model;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Models
{
    public class Division: LdapGroup
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(Division));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Division.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.Division;
        public new static readonly string[] LoadProperties = new[]
        {
            LdapProperties.DepartmentId
        }.Concat(LdapGroup.LoadProperties).ToArray();
        
        [JsonProperty("DepartmentId")]
        [LdapAttr(LdapProperties.DepartmentId, typeof(int))]
        public int DepartmentId { get; set; }
    }
}