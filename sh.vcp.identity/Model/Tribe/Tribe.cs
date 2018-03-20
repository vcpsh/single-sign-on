using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class Tribe: LdapGroup, ILdapModelWithChildren
    {
        public new static readonly string[] LoadProperties = LdapGroup.LoadProperties;
        [JsonProperty("Sl")]
        public TribeSl Sl { get; set; }
        [JsonProperty("Gs")]
        public TribeGs Gs { get; set; }
        [JsonProperty("Lr")]
        public TribeLr Lr { get; set; }
        [JsonProperty("Lv")]
        public TribeLv Lv { get; set; }
        public async Task LoadChildren(ILdapConnection connection, CancellationToken cancellationToken = default)
        {
            this.Sl = await connection.Read<TribeSl>($"cn={this.Id} Stammesleitung,{this.Dn}", cancellationToken);
            this.Gs = await connection.Read<TribeGs>($"cn={this.Id} Geschaeftsstelle,{this.Dn}", cancellationToken);
            this.Lr = await connection.Read<TribeLr>($"cn={this.Id} Landesrat,{this.Dn}", cancellationToken);
            this.Lv = await connection.Read<TribeLv>($"cn={this.Id} Landesversammlung,{this.Dn}", cancellationToken);
        }
    }
}