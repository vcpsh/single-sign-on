using System.Security.Claims;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Claims
{
    public class IsDivisionLgsClaim : Claim
    {
        public static string ClaimType => LdapClaims.IsDivisionLgsClaim;
        
        public IsDivisionLgsClaim(Division division) : base(ClaimType, division.Id) { }

        public IsDivisionLgsClaim(string divisionId) : base(ClaimType, divisionId) { }
    }
}