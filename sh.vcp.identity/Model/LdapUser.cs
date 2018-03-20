using System.Linq;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Model
{
    public class LdapUser: LdapModel
    {
        public new static readonly string[] LoadProperties = new string[]
        {
            LdapProperties.Uid,
        }.Concat(LdapModel.LoadProperties).ToArray();
        
        public string UserName { get; set; }

        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
            this.UserName = entry.GetAttribute(LdapProperties.Uid);
        }
    }
}