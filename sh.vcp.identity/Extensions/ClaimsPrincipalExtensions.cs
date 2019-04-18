using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using sh.vcp.identity.Claims;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        
        public static bool IsDivisionAdminForGroup(this ClaimsPrincipal user, LdapGroup group)
        {
            return user.HasClaim(IsDivisionLgsClaim.ClaimType, group.DivisionId);
        }

        /// <summary>
        /// Returns all divisions a user is member of.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetUserDivisions(this ClaimsPrincipal user)
        {
            return user.Claims
                .Where(claim => claim.Type == DivisionClaim.ClaimType)
                .Select(claim => claim.Value)
                .ToList();
        }

        /// <summary>
        /// Is a user member of a division?
        /// </summary>
        /// <param name="user"></param>
        /// <param name="divisionId"></param>
        /// <returns></returns>
        public static bool IsDivisionMember(this ClaimsPrincipal user, string divisionId)
        {
            return user.HasClaim(DivisionClaim.ClaimType, divisionId);
        }
    }
}
