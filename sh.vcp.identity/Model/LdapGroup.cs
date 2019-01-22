using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.identity.Model.Tribe;
using sh.vcp.identity.Utils;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models
{
    public class LdapGroup : LdapModel
    {
        public LdapGroup() : base()
        {
            this.DefaultObjectClasses.Add(LdapObjectTypes.Group);
        }

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
            TribeGroup,
        }

        protected new static readonly List<string> DefaultObjectClassesStatic =
            LdapModel.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.Group}).ToList();

        private static readonly Dictionary<PropertyInfo, LdapAttr> Props =
            LdapAttrHelper.GetLdapAttrs(typeof(LdapGroup));

        public new static readonly string[] LoadProperties = new[]
        {
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
        public List<string> MemberIds { get; set; } = new List<string>();

        [JsonProperty("DivisionId")]
        [Required]
        [DivisionIdValidation]
        public string DivisionId { get; set; }

        [JsonProperty("OfficialMail")]
        [LdapAttr(LdapProperties.OfficialMail, true)]
        [EmailAddress]
        public string OfficialMail { get; set; }

        [JsonProperty("Type")]
        public GroupType Type
        {
            get
            {
                if (this.ObjectClasses.Contains(LdapObjectTypes.VotedGroup))
                    return GroupType.VotedGroup;

                if (this.ObjectClasses.Contains(LdapObjectTypes.TribeLv))
                    return GroupType.TribeLv;

                if (this.ObjectClasses.Contains(LdapObjectTypes.TribeLr))
                    return GroupType.TribeLr;

                if (this.ObjectClasses.Contains(LdapObjectTypes.TribeSl))
                    return GroupType.TribeSl;

                if (this.ObjectClasses.Contains(LdapObjectTypes.TribeGs))
                    return GroupType.TribeGs;

                if (this.ObjectClasses.Contains(LdapObjectTypes.TribeGroup))
                    return GroupType.TribeGroup;

                if (this.ObjectClasses.Contains(LdapObjectTypes.Tribe))
                    return GroupType.Tribe;

                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (this.ObjectClasses.Contains(LdapObjectTypes.Division))
                    return GroupType.Division;

                return GroupType.Group;
            }
            set
            {
                switch (value) {
                    case GroupType.Group:
                        this.ObjectClasses.AddRange(LdapModel.DefaultObjectClassesStatic);
                        break;
                    case GroupType.Division:
                        this.ObjectClasses.AddRange(Division.DefaultObjectClassesStatic);
                        break;
                    case GroupType.VotedGroup:
                        this.ObjectClasses.AddRange(VotedLdapGroup.DefaultObjectClassesStatic);
                        break;
                    case GroupType.Tribe:
                        this.ObjectClasses.AddRange(Tribe.DefaultObjectClassesStatic);
                        break;
                    case GroupType.TribeGs:
                        this.ObjectClasses.AddRange(TribeGs.DefaultObjectClassesStatic);
                        break;
                    case GroupType.TribeSl:
                        this.ObjectClasses.AddRange(TribeSl.DefaultObjectClassesStatic);
                        break;
                    case GroupType.TribeLr:
                        this.ObjectClasses.AddRange(TribeLr.DefaultObjectClassesStatic);
                        break;
                    case GroupType.TribeLv:
                        this.ObjectClasses.AddRange(TribeLv.DefaultObjectClassesStatic);
                        break;
                    case GroupType.TribeGroup:
                        this.ObjectClasses.AddRange(TribeGroup.DefaultObjectClassesStatic);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        /// <summary>
        ///     Returns the division this group is in.
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
            if (this.DisplayName == null) this.DisplayName = this.Id;
            this.DivisionId = this.GetDivisionName();
        }
    }
}