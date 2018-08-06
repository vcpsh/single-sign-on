using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Model
{
    public class LdapUser : LdapModel
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr> Props = LdapAttrHelper.GetLdapAttrs(typeof(LdapUser));
        protected override Dictionary<PropertyInfo, LdapAttr> Properties => LdapUser.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.User;

        public new static readonly string[] LoadProperties = new string[]
        {
            LdapProperties.Uid,
            LdapProperties.Email,
            LdapProperties.EmailVerified,
        }.Concat(LdapModel.LoadProperties).ToArray();

        [JsonProperty("Username")]
        [LdapAttr(LdapProperties.Uid, true)]
        public string UserName { get; set; }

        [JsonProperty("Email")]
        [LdapAttr(LdapProperties.Email, true)]
        public string Email { get; set; }

        [JsonProperty("EmailVerified")]
        [LdapAttr(LdapProperties.EmailVerified, typeof(bool), true)]
        public bool EmailVerified { get; set; }

        protected override List<LdapModification> GetModifcationsList(List<LdapModification> list = null)
        {
            List<LdapModification> mods = base.GetModifcationsList(list);
            var previousEmail = this.Entry.GetOptionalAttribute(LdapProperties.Email);
            if (previousEmail == null)
            {
                mods.Add(
                    new LdapModification(LdapModification.ADD, new LdapAttribute(LdapProperties.Email, this.Email)));
            }
            else if (previousEmail != this.Email)
            {
                mods.Add(new LdapModification(LdapModification.REPLACE,
                    new LdapAttribute(LdapProperties.Email, this.Email)));
            }

            if (this.Entry.getAttribute(LdapProperties.EmailVerified) == null)
            {
                mods.Add(new LdapModification(LdapModification.ADD,
                    new LdapAttribute(LdapProperties.EmailVerified, this.EmailVerified ? "TRUE" : "FALSE")));
            }
            else if (this.Entry.GetOptionalBoolAttribute(LdapProperties.EmailVerified) != this.EmailVerified)
            {
                mods.Add(new LdapModification(LdapModification.REPLACE,
                    new LdapAttribute(LdapProperties.EmailVerified, this.EmailVerified ? "TRUE" : "FALSE")));
            }

            if (this.Entry.GetOptionalAttribute(LdapProperties.Uid) == null)
            {
                mods.Add(new LdapModification(LdapModification.ADD,
                    new LdapAttribute(LdapProperties.Uid, this.UserName)));
            }
            else if (this.Entry.GetOptionalAttribute(LdapProperties.Uid) != this.UserName)
            {
                mods.Add(new LdapModification(LdapModification.REPLACE,
                    new LdapAttribute(LdapProperties.Uid, this.UserName)));
            }

            return mods;
        }
    }
}