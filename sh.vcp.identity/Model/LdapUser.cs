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

        protected override List<LdapModification> GetModifcationsList(List<LdapModification> list = null)
        {
            List<LdapModification> mods = base.GetModifcationsList(list);
            var previousEmail = this._entry.GetOptionalAttribute(LdapProperties.Email);
            if (previousEmail == null)
            {
                mods.Add(new LdapModification(LdapModification.ADD, new LdapAttribute(LdapProperties.Email, this.Email)));
            } else if (previousEmail != this.Email)
            {
                mods.Add(new LdapModification(LdapModification.REPLACE, new LdapAttribute(LdapProperties.Email, this.Email)));
            }

            if (this._entry.getAttribute(LdapProperties.EmailVerified) == null)
            {
                mods.Add(new LdapModification(LdapModification.ADD, new LdapAttribute(LdapProperties.EmailVerified, this.EmailVerified ? "TRUE" : "FALSE")));
            } else if (this._entry.GetOptionalBoolAttribute(LdapProperties.EmailVerified) != this.EmailVerified)
            {
                mods.Add(new LdapModification(LdapModification.REPLACE, new LdapAttribute(LdapProperties.EmailVerified, this.EmailVerified ? "TRUE" : "FALSE")));
            }

            if (this._entry.GetOptionalAttribute(LdapProperties.Uid) == null)
            {
                mods.Add(new LdapModification(LdapModification.ADD, new LdapAttribute(LdapProperties.Uid, this.UserName)));
            } else if (this._entry.GetOptionalAttribute(LdapProperties.Uid) != this.UserName)
            {
                mods.Add(new LdapModification(LdapModification.REPLACE, new LdapAttribute(LdapProperties.Uid, this.UserName)));
            }
            return mods;
        }
    }
}