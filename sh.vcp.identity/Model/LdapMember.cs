using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Model
{
    public class LdapMember : LdapUser
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(LdapMember));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => LdapMember.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.Member;

        public new static readonly string[] LoadProperties = new string[]
        {
            LdapProperties.FirstName,
            LdapProperties.LastName,
            LdapProperties.DateOfBirth,
            LdapProperties.AccessionDate,
            LdapProperties.Gender,
        }.Concat(LdapModel.LoadProperties).ToArray();

        [JsonProperty("FirstName")]
        [LdapAttr(LdapProperties.FirstName)]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        [LdapAttr(LdapProperties.LastName)]
        public string LastName { get; set; }

        [JsonProperty("DateOfBirth")]
        [LdapAttr(LdapProperties.DateOfBirth, typeof(DateTime))]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("AccessionDate")]
        [LdapAttr(LdapProperties.AccessionDate, typeof(DateTime))]
        public DateTime AccessionDate { get; set; }

        [JsonProperty("Gender")]
        [LdapAttr(LdapProperties.Gender)]
        public string Gender { get; set; }
    }
}