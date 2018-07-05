using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.identity.Models;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;
using ILdapConnection = sh.vcp.ldap.ILdapConnection;

namespace sh.vcp.identity.Model.Tribe
{
    public class Tribe: LdapGroup, ILdapModelWithChildren
    {
        protected override string __defaultObjectClass => LdapObjectTypes.Tribe;
        
        public new static readonly string[] LoadProperties = new string[]
        {
            LdapProperties.DepartmentId,
        }.Concat(LdapGroup.LoadProperties).ToArray();
        
        [JsonProperty("Sl")]
        public TribeSl Sl { get; set; }
        [JsonProperty("Gs")]
        public TribeGs Gs { get; set; }
        [JsonProperty("Lr")]
        public TribeLr Lr { get; set; }
        [JsonProperty("Lv")]
        public TribeLv Lv { get; set; }
        
        [JsonProperty("DepartmentId")]
        public int DepartmentId { get; set; }
        
        public async Task LoadChildren(ILdapConnection connection, CancellationToken cancellationToken = default)
        {
            this.Sl = await connection.Read<TribeSl>($"cn={this.Id}_sl,{this.Dn}", cancellationToken);
            this.Gs = await connection.Read<TribeGs>($"cn={this.Id}_gs,{this.Dn}", cancellationToken);
            this.Lr = await connection.Read<TribeLr>($"cn={this.Id}_lr,{this.Dn}", cancellationToken);
            this.Lv = await connection.Read<TribeLv>($"cn={this.Id}_lv,{this.Dn}", cancellationToken);
        }

        public ICollection<LdapModel> GetChildren()
        {
            return new List<LdapModel>
            {
                this.Gs, this.Lr, this.Lv, this.Sl
            };
        }

        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
            this.DepartmentId = entry.GetAttributeInt(LdapProperties.DepartmentId);
        }

        public override LdapAttributeSet GetAttributeSet(LdapAttributeSet set = null)
        {
            return base.GetAttributeSet(set)
                .Add(LdapProperties.DepartmentId, this.DepartmentId);
        }
    }
}