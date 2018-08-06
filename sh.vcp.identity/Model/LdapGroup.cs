using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.identity.Model;
using sh.vcp.identity.Utils;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Models
{
    public class LdapGroup : LdapModel
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(LdapGroup));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => LdapGroup.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.Group;

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
        [LdapAttr(LdapProperties.DisplayName, true)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Ids of the members in the group.
        /// </summary>
        [JsonProperty("MemberIds")]
        [LdapAttr(LdapProperties.Member, typeof(List<string>), true)]
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
            if (this.DisplayName == null)
            {
                this.DisplayName = this.Id;
            }
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

        protected override List<LdapModification> GetModifcationsList(List<LdapModification> list = null)
        {
            List<LdapModification> mods = base.GetModifcationsList(list);
            List<string> oldMemberIds = this.Entry.GetOptionalListAttribute(LdapProperties.Member);
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