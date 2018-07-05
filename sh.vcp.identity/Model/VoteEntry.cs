using System;
using System.Globalization;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Model
{
    public class VoteEntry: LdapModel
    {
        protected override string __defaultObjectClass => LdapObjectTypes.VotedEntry;
        public new static readonly string[] LoadProperties = new[]
        {
            LdapProperties.Active,
            LdapProperties.VoteStartDate,
            LdapProperties.VoteStartEvent,
            LdapProperties.VoteEndDate,
            LdapProperties.VoteEndEvent,
        };

        [JsonProperty("Active")]
        public bool Active { get; set; }
        
        [JsonProperty("VoteStartDate")]
        public DateTime VoteStartDate { get; set; }
        
        [JsonProperty("VoteEndDate")]
        public DateTime VoteEndDate { get; set; }
        
        [JsonProperty("VoteStartEvent")]
        public string VoteStartEvent { get; set; }
        
        [JsonProperty("VoteEndEvent")]
        public string VoteEndEvent { get; set; }

        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
            this.Active = entry.GetAttribute(LdapProperties.Active) == "1";
            this.VoteStartDate = DateTime.ParseExact(entry.GetAttribute(LdapProperties.VoteStartDate), LdapConstants.DateFormat, CultureInfo.InvariantCulture);
            var voteEndDate = entry.GetOptionalAttribute(LdapProperties.VoteEndDate);
            if (voteEndDate != null)
            {
                this.VoteEndDate = DateTime.ParseExact(voteEndDate, LdapConstants.DateFormat, CultureInfo.InvariantCulture);
            }
            this.VoteStartEvent = entry.GetAttribute(LdapProperties.VoteStartEvent);
            this.VoteEndEvent = entry.GetOptionalAttribute(LdapProperties.VoteEndEvent);

        }
    }
}