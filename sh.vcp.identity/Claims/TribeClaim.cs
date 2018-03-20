using System.Security.Claims;
using sh.vcp.identity.Model;
using sh.vcp.identity.Model.Tribe;
using sh.vcp.identity.Models;

namespace sh.vcp.identity.Claims
{
    public class TribeClaim: Claim
    {
        internal readonly Tribe Tribe;
        public const string ClaimId = LdapClaims.TribeClaim;
        public TribeClaim(Tribe tribe) : base(TribeClaim.ClaimId, tribe.Id)
        {
            this.Tribe = tribe;
        }
    }
}