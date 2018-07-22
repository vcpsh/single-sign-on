using System.Text.RegularExpressions;
using Novell.Directory.Ldap;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeLr: LdapGroup
    {
        protected override string __defaultObjectClass => LdapObjectTypes.TribeLr;
        
        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
        }
    }
}