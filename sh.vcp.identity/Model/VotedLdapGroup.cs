using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.identity.Model;
using sh.vcp.ldap;
using ILdapConnection = sh.vcp.ldap.ILdapConnection;

namespace sh.vcp.identity.Models
{
    public class VotedLdapGroup : LdapGroup, ILdapModelWithChildren
    {
        public VotedLdapGroup() : base() {
            this.DefaultObjectClasses.Add(LdapObjectTypes.VotedGroup);
        }

        protected new static readonly List<string> DefaultObjectClassesStatic =
            LdapGroup.DefaultObjectClassesStatic.Concat(new List<string> {LdapObjectTypes.VotedGroup}).ToList();
        
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props =
            LdapAttrHelper.GetLdapAttrs(typeof(VotedLdapGroup));

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => VotedLdapGroup.Props;

        [JsonProperty("activeVoteEntries")]
        public ICollection<VoteEntry> ActiveVoteEntries { get; set; } = new List<VoteEntry>();

        [JsonProperty("inactiveVoteEntries")]
        public ICollection<VoteEntry> InactiveVoteEntries { get; set; } = new List<VoteEntry>();

        public async Task LoadChildren(ILdapConnection connection, CancellationToken cancellationToken = default) {
            ICollection<VoteEntry> entries = await connection.Search<VoteEntry>(this.Dn, null,
                LdapObjectTypes.VotedEntry, LdapConnection.SCOPE_ONE, VoteEntry.LoadProperties, cancellationToken);
            foreach (var voteEntry in entries)
                if (voteEntry.Active)
                    this.ActiveVoteEntries.Add(voteEntry);
                else
                    this.InactiveVoteEntries.Add(voteEntry);
        }

        public IEnumerable<LdapModel> GetChildren() {
            return (ICollection<LdapModel>) this.ActiveVoteEntries.Concat(this.InactiveVoteEntries);
        }
    }
}