using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models
{
        public class OrgUnit : LdapGroup
        {
            public OrgUnit() : base() {
                this.DefaultObjectClasses.Add(LdapObjectTypes.OrgUnit);
            }

            protected static new readonly List<string> DefaultObjectClassesStatic =
                LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.OrgUnit}).ToList();
        
            private static readonly Dictionary<PropertyInfo, LdapAttr>
                Props = LdapAttrHelper.GetLdapAttrs(typeof(OrgUnit));

            public static new readonly string[] LoadProperties = LdapGroup.LoadProperties;

            protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;

            [JsonProperty("orgUnitType")]
            public OrgUnitTypeEnum OrgUnitType
            {
                get
                {
                    switch (this.Id) {
                        case "events":
                            return OrgUnitTypeEnum.Events;
                        case "groups":
                            return OrgUnitTypeEnum.Groups;
                        case "tribes":
                            return OrgUnitTypeEnum.Tribes;
                        case "voted_groups":
                            return OrgUnitTypeEnum.VotedGroups;
                        default:
                            throw new ArgumentOutOfRangeException($"invalid id ({this.Id} for org unit");
                    }
                }
            }

            public enum OrgUnitTypeEnum
            {
                Groups,
                Events,
                Tribes,
                VotedGroups,
            }
        }
}
