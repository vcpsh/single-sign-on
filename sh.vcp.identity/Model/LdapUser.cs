using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Model
{
    public class LdapUser: LdapModel
    {
        protected override string __defaultObjectClass => LdapObjectTypes.User;
        public new static readonly string[] LoadProperties = new string[]
        {
            LdapProperties.Uid,
            LdapProperties.Email,
            LdapProperties.EmailVerified,
        }.Concat(LdapModel.LoadProperties).ToArray();
        
        [JsonProperty("Username")]
        public string UserName { get; set; }
        
        [JsonProperty("Email")]
        public string Email { get; set; }
        
        [JsonProperty("EmailVerified")]
        public bool EmailVerified { get; set; }

        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
            this.UserName = entry.GetOptionalAttribute(LdapProperties.Uid);
            this.Email = entry.GetOptionalAttribute(LdapProperties.Email);
            this.EmailVerified = entry.GetOptionalBoolAttribute(LdapProperties.EmailVerified);
        }

        public override LdapAttributeSet GetAttributeSet(LdapAttributeSet set = null)
        {
            return base.GetAttributeSet(set)
                .AddOptional(LdapProperties.Uid, this.UserName)
                .AddOptional(LdapProperties.Email, this.Email)
                .AddOptional(LdapProperties.EmailVerified, this.EmailVerified);
        }
    }
}