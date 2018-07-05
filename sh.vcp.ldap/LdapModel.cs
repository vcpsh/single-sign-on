using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.ldap
{
    public abstract class LdapModel
    {
        protected virtual string __defaultObjectClass => string.Empty;
        public static readonly string[] LoadProperties = new[]
        {
            LdapProperties.CommonName,
            LdapProperties.ObjectClass
        };

        protected LdapEntry _entry { get; set; }

        private string _objectClass;
        
        /// <summary>
        /// Id of the object (cn=*).
        /// </summary>
        [JsonProperty("Id")]
        [Required]
        public string Id { get; set; }
        
        /// <summary>
        /// Domainname of the object.
        /// </summary>
        [JsonProperty("Dn")]
        [Required]
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
                .Add(LdapProperties.ObjectClass, this._objectClass ?? this.__defaultObjectClass)
                .Add(LdapProperties.CommonName, this.Id);
        }

        protected virtual List<LdapModification> GetModifcationsList(List<LdapModification> list = null)
        {
            var modifications = new List<LdapModification>();
            return modifications;
        }

        public LdapModification[] GetModifications()
        {
            return this.GetModifcationsList().ToArray();
        }

        public LdapEntry ToEntry()
        {
            return new LdapEntry(this.Dn, this.GetAttributeSet());
        }
    }
}