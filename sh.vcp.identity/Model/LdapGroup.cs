using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.identity.Utils;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Models
{
    public class LdapGroup : LdapModel
    {
        protected override string __defaultObjectClass => LdapObjectTypes.Group;

        public new static readonly string[] LoadProperties = new[]
        {
            LdapProperties.Member,
            LdapProperties.DisplayName,
        }.Concat(LdapModel.LoadProperties).ToArray();

        /// <summary>
        /// DisplayName of the group. Should be used in the uid.
        /// </summary>
        [JsonProperty("DisplayName")]
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// Ids of the members in the group.
        /// </summary>
        [JsonProperty("MemberIds")]
        public List<string> MemberIds { get; set; }

        [JsonProperty("DivisionId")]
        [Required]
        [DivisionIdValidation]
        public string DivisionId { get; set; }
        
        [JsonProperty("Type")]
        public GroupType Type { get; set; }

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
            this.DisplayName = entry.GetOptionalAttribute(LdapProperties.DisplayName) ?? this.Id;
            this.MemberIds = entry.GetOptionalListAttribute(LdapProperties.Member);
            this.DivisionId = this.GetDivisionName();
            switch (this.ObjectClass)
            {
                case LdapObjectTypes.Division:
                    this.Type = GroupType.Division;
                    break;
                case LdapObjectTypes.Tribe:
                    this.Type = GroupType.Tribe;
                    break;
                case LdapObjectTypes.TribeGs:
                    this.Type = GroupType.TribeGs;
                    break;
                case LdapObjectTypes.TribeSl:
                    this.Type = GroupType.TribeSl;
                    break;
                case LdapObjectTypes.TribeLr:
                    this.Type = GroupType.TribeLr;
                    break;
                case LdapObjectTypes.TribeLv:
                    this.Type = GroupType.TribeLv;
                    break;
                case LdapObjectTypes.VotedGroup:
                    this.Type = GroupType.VotedGroup;
                    break;
                default:
                    this.Type = GroupType.Group;
                    break;
            }
        }

        public override LdapAttributeSet GetAttributeSet(LdapAttributeSet set = null)
        {
            return base.GetAttributeSet(set)
                .AddOptional(LdapProperties.DisplayName, this.DisplayName)
                .AddOptional(LdapProperties.Member, this.MemberIds);
        }

        protected override List<LdapModification> GetModifcationsList(List<LdapModification> list = null)
        {
            List<LdapModification> mods = base.GetModifcationsList(list);
            List<string> oldMemberIds = this._entry.GetOptionalListAttribute(LdapProperties.Member);
            var intersectCount = oldMemberIds.Intersect(this.MemberIds).Count();
            if (intersectCount != oldMemberIds.Count || intersectCount != this.MemberIds.Count)
            {
                // find added member ids
                this.MemberIds.ForEach(m =>
                {
                    if (oldMemberIds.Contains(m)) return;
                    var mod = new LdapModification(LdapModification.ADD, new LdapAttribute(LdapProperties.Member, m));
                    mods.Add(mod);
                });
                // find removed member ids
                oldMemberIds.ForEach(m =>
                {
                    if (this.MemberIds.Contains(m)) return;
                    var mod = new LdapModification(LdapModification.DELETE,
                        new LdapAttribute(LdapProperties.Member, m));
                    mods.Add(mod);
                });
            }

            return mods;
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
            TribeLv,
        }
    }
}