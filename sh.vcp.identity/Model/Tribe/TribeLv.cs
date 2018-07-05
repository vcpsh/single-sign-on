using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeLv: LdapGroup
    {
        protected override string __defaultObjectClass => LdapObjectTypes.TribeLv;
        
    }
}