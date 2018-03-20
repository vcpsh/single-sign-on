using System.Security.Claims;
using sh.vcp.identity.Model.Tribe;

namespace sh.vcp.identity.Claims
{
    public class IsTribeSlClaim: Claim
    {
        internal readonly Tribe Tribe;

        public IsTribeSlClaim(Tribe tribe) : base(LdapClaims.IsTribeSlClaim, tribe.Id)
        {
            this.Tribe = tribe;
        }
    }
}