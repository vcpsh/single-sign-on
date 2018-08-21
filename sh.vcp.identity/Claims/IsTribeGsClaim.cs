using System.Security.Claims;
using sh.vcp.identity.Model.Tribe;

namespace sh.vcp.identity.Claims
{
    public class IsTribeGsClaim : Claim
    {
        internal readonly Tribe Tribe;

        public IsTribeGsClaim(Tribe tribe) : base(LdapClaims.IsTribeGsClaim, tribe.Id) {
            this.Tribe = tribe;
        }
    }
}