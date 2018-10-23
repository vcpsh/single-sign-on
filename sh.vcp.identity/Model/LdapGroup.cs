using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.identity.Utils;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models
{
    public class LdapGroup : LdapModel
    {
        public LdapGroup() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.Group);
        }

        /// <summary>
        /// Workaround group type, to keep the group type on transfer.
        /// </summary>
        public enum GroupType
        {
            Group,
            Division,
            VotedGroup,
            Tribe,
            TribeGs,
            TribeSl,
            TribeLr,
            TribeLv
        }

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props =
            LdapAttrHelper.GetLdapAttrs(typeof(LdapGroup));

        public new static readonly string[] LoadProperties = new[] {
            LdapProperties.Member,
            LdapProperties.DisplayName,
            LdapProperties.OfficialMail,
        }.Concat(LdapModel.LoadProperties).ToArray();

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => LdapGroup.Props;

        /// <summary>
        ///     DisplayName of the group. Should be used in the uid.
        /// </summary>
        [JsonProperty("DisplayName")]
        [Required]
        [LdapAttr(LdapProperties.DisplayName, true)]
        public string DisplayName { get; set; }

        /// <summary>
        ///     Ids of the members in the group.
        /// </summary>
        [JsonProperty("MemberIds")]
        [LdapAttr(LdapProperties.Member, typeof(List<string>), true)]
        public List<string> MemberIds { get; set; }

        [JsonProperty("DivisionId")]
        [Required]
        [DivisionIdValidation]
        public string DivisionId { get; set; }

        [JsonProperty("OfficialMail")]
        [LdapAttr(LdapProperties.OfficialMail, true)]
        [EmailAddress]
        public string OfficialMail { get; set; }

        [JsonProperty("Type")]
        public GroupType Type { get; set; }

        /// <summary>
        ///     Returns the division this group is in.
        /// </summary>
        /// <returns></returns>
        private string GetDivisionName() {
            string[] dnParts = this.Dn.Split(',');
            return dnParts.Length - 4 < 0 ? "" : dnParts[dnParts.Length - 4].Substring(3);
        }

        public override void ProvideEntry(LdapEntry entry) {
            base.ProvideEntry(entry);
            if (this.DisplayName == null) this.DisplayName = this.Id;
            this.DivisionId = this.GetDivisionName();
            // Maybe this is not needed anymore, because we send the object class over the wire
//            this.Type = GroupType.Group;
//
//            if (this.ObjectClasses.Contains(LdapObjectTypes.VotedGroup)) {
//                this.ObjectClasses.Add(LdapObjectTypes.VotedGroup);
//            }
//            else if (this.ObjectClasses.Contains(LdapObjectTypes.TribeLv)) {
//                this.ObjectClasses.Add(LdapObjectTypes.TribeLv);
//            }
//            else if (this.ObjectClasses.Contains(LdapObjectTypes.TribeLr)) {
//                this.ObjectClasses.Add(LdapObjectTypes.TribeLr);
//            }
//            else if (this.ObjectClasses.Contains(LdapObjectTypes.TribeSl)) {
//                this.ObjectClasses.Add(LdapObjectTypes.TribeSl);
//            }
//            else if (this.ObjectClasses.Contains(LdapObjectTypes.TribeGs)) {
//                this.ObjectClasses.Add(LdapObjectTypes.TribeGs);
//            }
//            else if (this.ObjectClasses.Contains(LdapObjectTypes.Tribe)) {
//                this.ObjectClasses.Add(LdapObjectTypes.Tribe);
//            }
//            else if (this.ObjectClasses.Contains(LdapObjectTypes.Division)) {
//                this.ObjectClasses.Add(LdapObjectTypes.Division);
//            }
        }
    }
}