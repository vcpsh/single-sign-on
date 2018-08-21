using System.Security.Claims;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Claims
{
    public class DivisionClaim : Claim
    {
        internal readonly Division Division;

        public DivisionClaim(Division division) : base(LdapClaims.DivisionClaim, division.Id) {
            this.Division = division;
        }
    }
}