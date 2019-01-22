using System.Security.Claims;
using sh.vcp.identity.Claims;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsDivisionAdminForGroup(this ClaimsPrincipal claims, LdapGroup group)
        {
            return claims.HasClaim(IsDivisionLgsClaim.ClaimType, group.DivisionId);
        }
    }
}