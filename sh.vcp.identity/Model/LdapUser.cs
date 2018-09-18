using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model
{
    public class LdapUser : LdapModel
    {
        private static readonly Dictionary<PropertyInfo, LdapAttr>
            Props = LdapAttrHelper.GetLdapAttrs(typeof(LdapUser));

        public new static readonly string[] LoadProperties = new[] {
            LdapProperties.Uid,
            LdapProperties.Email,
            LdapProperties.EmailVerified
        }.Concat(LdapModel.LoadProperties).ToArray();

        protected override Dictionary<PropertyInfo, LdapAttr> Properties => LdapUser.Props;
        protected override string DefaultObjectClass => LdapObjectTypes.User;

        [JsonProperty("Username")]
        [LdapAttr(LdapProperties.Uid, true)]
        public string UserName { get; set; }

        [JsonProperty("Email")]
        [LdapAttr(LdapProperties.Email, true)]
        public string Email { get; set; }

        [JsonProperty("EmailVerified")]
        [LdapAttr(LdapProperties.EmailVerified, typeof(bool), true)]
        public bool EmailVerified { get; set; }
    }
}