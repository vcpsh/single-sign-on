using System.Security.Claims;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Claims
{
    /// <inheritdoc />
    /// <summary>
    /// Claim for division membership.
    /// </summary>
    public class DivisionClaim : Claim
    {
        public static string ClaimType => LdapClaims.DivisionClaim;
        private readonly Division Division;

        public DivisionClaim(Division division) : base(DivisionClaim.ClaimType, division.Id) {
            this.Division = division;
        }
    }
}