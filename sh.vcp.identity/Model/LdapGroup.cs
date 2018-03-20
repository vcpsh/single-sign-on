using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Models
{
    public class LdapGroup : LdapModel
    {
        public new static readonly string[] LoadProperties = new[]
        {
            LdapProperties.Member,
            LdapProperties.DisplayName,
        }.Concat(LdapModel.LoadProperties).ToArray();

        /// <summary>
        /// DisplayName of the group. Should be used in the uid.
        /// </summary>
        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Ids of the members in the group.
        /// </summary>
        [JsonProperty("MemberIds")]
        public List<string> MemberIds { get; set; }
        
        [JsonProperty("DivisionId")]
        public string DivisionId { get; set; }

        /// <summary>
        /// Returns the division this group is in.
        /// </summary>
        /// <returns></returns>
        private string GetDivisionName()
        {
            string[] dnParts = this.Dn.Split(',');
            return dnParts.Length - 4 < 0 ? "" : dnParts[dnParts.Length - 4].Substring(3);
        }

        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
            this.DisplayName = entry.GetOptionalAttribute(LdapProperties.DisplayName);
            this.MemberIds = entry.GetOptionalListAttribute(LdapProperties.Member);
            this.DivisionId = this.GetDivisionName();
        }

        public override LdapAttributeSet GetAttributeSet(LdapAttributeSet set = null)
        {
            return base.GetAttributeSet(set)
                .AddOptional(LdapProperties.DisplayName, this.DisplayName)
                .AddOptional(LdapProperties.Member, this.MemberIds);
        }
    }
}