using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.ldap
{
    public abstract class LdapModel
    {
        public static readonly string[] LoadProperties = new[]
        {
            LdapProperties.CommonName,
            LdapProperties.ObjectClass
        };
        
        private LdapEntry _entry { get; set; }
        
        private string _objectClass;
        /// <summary>
        /// Id of the object (cn=*).
        /// </summary>
        [JsonProperty("Id")]
        public string Id { get; set; }
        /// <summary>
        /// Domainname of the object.
        /// </summary>
        [JsonProperty("Dn")]
        public string Dn { get; set; }

        /// <summary>
        /// Converts a ldap entry to the ldap model object.
        /// </summary>
        /// <param name="entry">Entry to convert.</param>
        public virtual void ProvideEntry(LdapEntry entry)
        {
            this._objectClass = entry.GetAttribute(LdapProperties.ObjectClass);
            this.Id = entry.GetAttribute(LdapProperties.CommonName);
            this.Dn = entry.DN;
            this._entry = entry;
        }

        /// <summary>
        /// Convertas a ldap model to the corresponding ldap attribute set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public virtual LdapAttributeSet GetAttributeSet(LdapAttributeSet set = null)
        {
            return new LdapAttributeSet()
                .Add(LdapProperties.ObjectClass, this._objectClass)
                .Add(LdapProperties.CommonName, this.Id);
        }

        public LdapEntry ToEntry(LdapEntry entry)
        {
            return new LdapEntry(this.Dn, this.GetAttributeSet());
        }
    }
}