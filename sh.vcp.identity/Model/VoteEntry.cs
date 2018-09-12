using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model
{
    public class VoteEntry : LdapModel
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props =
            LdapAttrHelper.GetLdapAttrs(typeof(VoteEntry));

        public new static readonly string[] LoadProperties = new[] {
            LdapProperties.Active,
            LdapProperties.VoteStartDate,
            LdapProperties.VoteStartEvent,
            LdapProperties.VoteEndDate,
            LdapProperties.VoteEndEvent,
            LdapProperties.Member,
        }.Concat(LdapModel.LoadProperties).ToArray();

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => VoteEntry.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.VotedEntry;

        [JsonProperty("MemberId")]
        [LdapAttr(LdapProperties.Member)]
        public string MemberUid { get; set; }
        
        [JsonProperty("Active")]
        [LdapAttr(LdapProperties.Active, typeof(bool))]
        public bool Active { get; set; }

        [JsonProperty("VoteStartDate")]
        [LdapAttr(LdapProperties.VoteStartDate, typeof(DateTime))]
        public DateTime VoteStartDate { get; set; }

        [JsonProperty("VoteEndDate")]
        [LdapAttr(LdapProperties.VoteEndDate, typeof(DateTime), true)]
        public DateTime? VoteEndDate { get; set; }

        [JsonProperty("VoteStartEvent")]
        [LdapAttr(LdapProperties.VoteStartEvent)]
        public string VoteStartEvent { get; set; }

        [JsonProperty("VoteEndEvent")]
        [LdapAttr(LdapProperties.VoteEndEvent, true)]
        public string VoteEndEvent { get; set; }
    }
}