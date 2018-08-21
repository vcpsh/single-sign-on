using System.Security.Claims;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Claims
{
    public class IsDivisionLgsClaim : Claim
    {
        public IsDivisionLgsClaim(Division division) : base(LdapClaims.IsDivisionLgsClaim, division.Id) {
        }

        public IsDivisionLgsClaim(string divisionId) : base(LdapClaims.IsDivisionLgsClaim, divisionId) {
        }
    }
}