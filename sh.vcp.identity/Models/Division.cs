using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models
{
    public class Division : LdapGroup, ILdapModelWithChildren
    {
        public Division() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.Division);
        }

        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.Division}).ToList();
        
        private static readonly Dictionary<PropertyInfo, LdapAttr>
            Props = LdapAttrHelper.GetLdapAttrs(typeof(Division));

        public static new readonly string[] LoadProperties = new[] {
            LdapProperties.DepartmentId
        }.Concat(LdapGroup.LoadProperties).ToArray();

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;

        [JsonProperty("departmentId")]
        [LdapAttr(LdapProperties.DepartmentId, typeof(int))]
        public int DepartmentId { get; set; }
        
        [JsonProperty("events")]
        public OrgUnit Events { get; set; }
        
        [JsonProperty("groups")]
        public OrgUnit Groups { get; set; }
        
        [JsonProperty("tribes")]
        public OrgUnit Tribes { get; set; }
        
        [JsonProperty("votedGroups")]
        public OrgUnit VotedGroups { get; set; }

        public async Task LoadChildren(ILdapConnection connection, CancellationToken cancellationToken)
        {
            this.Events = await connection.ReadSafe<OrgUnit>($"cn=events,{this.Dn}", cancellationToken);
            this.Groups = await connection.ReadSafe<OrgUnit>($"cn=groups,{this.Dn}", cancellationToken);
            this.Tribes = await connection.ReadSafe<OrgUnit>($"cn=tribes,{this.Dn}", cancellationToken);
            this.VotedGroups = await connection.ReadSafe<OrgUnit>($"cn=voted_groups,{this.Dn}", cancellationToken);
        }

        public IEnumerable<LdapModel> GetChildren()
        {
            return new[]
            {
                this.Events,
                this.Groups,
                this.Tribes,
                this.VotedGroups,
            };
        }
    }
}
