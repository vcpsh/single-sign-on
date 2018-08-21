using System.Security.Claims;
using sh.vcp.identity.Model.Tribe;

namespace sh.vcp.identity.Claims
{
    public class TribeClaim : Claim
    {
        public const string ClaimId = LdapClaims.TribeClaim;
        internal readonly Tribe Tribe;

        public TribeClaim(Tribe tribe) : base(TribeClaim.ClaimId, tribe.Id) {
            this.Tribe = tribe;
        }
    }
}