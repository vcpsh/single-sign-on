using System.Text.RegularExpressions;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeLr: LdapGroup
    {
        protected override string __defaultObjectClass => LdapObjectTypes.TribeLr;
        
    }
}