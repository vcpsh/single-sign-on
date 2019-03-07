using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models
{
    public class VoteEntry : LdapModel
    {
        public VoteEntry() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.VotedEntry);
        }
        
        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapModel.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.VotedEntry}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props =
            LdapAttrHelper.GetLdapAttrs(typeof(VoteEntry));

        public static new readonly string[] LoadProperties = new[] {
            LdapProperties.Active,
            LdapProperties.VoteStartDate,
            LdapProperties.VoteStartEvent,
            LdapProperties.VoteEndDate,
            LdapProperties.VoteEndEvent,
            LdapProperties.Member,
        }.Concat(LdapModel.LoadProperties).ToArray();

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;

        [JsonProperty("memberId")]
        [LdapAttr(LdapProperties.Member)]
        public string MemberUid { get; set; }

        [JsonProperty("active")]
        [LdapAttr(LdapProperties.Active, typeof(bool))]
        public bool Active { get; set; }

        [JsonProperty("voteStartDate")]
        [LdapAttr(LdapProperties.VoteStartDate, typeof(DateTime))]
        public DateTime VoteStartDate { get; set; }

        [JsonProperty("voteEndDate")]
        [LdapAttr(LdapProperties.VoteEndDate, typeof(DateTime), true)]
        public DateTime? VoteEndDate { get; set; }

        [JsonProperty("voteStartEvent")]
        [LdapAttr(LdapProperties.VoteStartEvent)]
        public string VoteStartEvent { get; set; }

        [JsonProperty("voteEndEvent")]
        [LdapAttr(LdapProperties.VoteEndEvent, true)]
        public string VoteEndEvent { get; set; }
    }
}
