using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using sh.vcp.identity.Model;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models
{
    public class LdapMember : LdapUser
    {
        public LdapMember() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.Member);
        }
        
        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapUser.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.Member}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props =
            LdapAttrHelper.GetLdapAttrs(typeof(LdapMember));

        public static new readonly string[] LoadProperties = new[] {
            LdapProperties.FirstName,
            LdapProperties.LastName,
            LdapProperties.DateOfBirth,
            LdapProperties.AccessionDate,
            LdapProperties.Gender,
            LdapProperties.OfficialMail,
        }.Concat(LdapUser.LoadProperties).ToArray();

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;

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

        [JsonProperty("OfficialMail")]
        [LdapAttr(LdapProperties.OfficialMail, true)]
        [EmailAddress]
        public string OfficialMail { get; set; }
    }
}
