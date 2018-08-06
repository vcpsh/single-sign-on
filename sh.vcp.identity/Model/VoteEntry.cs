using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Model
{
    public class VoteEntry: LdapModel
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(VoteEntry));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => VoteEntry.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.VotedEntry;
        public new static readonly string[] LoadProperties = new[]
        {
            LdapProperties.Active,
            LdapProperties.VoteStartDate,
            LdapProperties.VoteStartEvent,
            LdapProperties.VoteEndDate,
            LdapProperties.VoteEndEvent,
        };

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