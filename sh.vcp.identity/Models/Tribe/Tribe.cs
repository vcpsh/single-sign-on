using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using sh.vcp.ldap;

namespace sh.vcp.identity.Models.Tribe
{
    public class Tribe : LdapGroup, ILdapModelWithChildren
    {
        public Tribe() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.Tribe);
        }

        protected static new readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.Tribe}).ToList();
        
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(Tribe));

        public static new readonly string[] LoadProperties = new[] {
            LdapProperties.DepartmentId
        }.Concat(LdapGroup.LoadProperties).ToArray();

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => Props;

        [JsonProperty("sl")]
        public TribeSl Sl { get; set; }

        [JsonProperty("gs")]
        public TribeGs Gs { get; set; }

        [JsonProperty("lr")]
        public TribeLr Lr { get; set; }

        [JsonProperty("lv")]
        public TribeLv Lv { get; set; }

        [JsonProperty("tribeId")]
        [LdapAttr(LdapProperties.DepartmentId, typeof(int))]
        public int DepartmentId { get; set; }

        public async Task LoadChildren(ILdapConnection connection, CancellationToken cancellationToken = default) {
            this.Sl = await connection.ReadSafe<TribeSl>($"cn={this.Id}_sl,{this.Dn}", cancellationToken);
            this.Gs = await connection.ReadSafe<TribeGs>($"cn={this.Id}_gs,{this.Dn}", cancellationToken);
            this.Lr = await connection.ReadSafe<TribeLr>($"cn={this.Id}_lr,{this.Dn}", cancellationToken);
            this.Lv = await connection.ReadSafe<TribeLv>($"cn={this.Id}_lv,{this.Dn}", cancellationToken);
        }

        public IEnumerable<LdapModel> GetChildren() {
            return new List<LdapModel> {
                this.Gs,
                this.Lr,
                this.Lv,
                this.Sl
            };
        }
    }
}
