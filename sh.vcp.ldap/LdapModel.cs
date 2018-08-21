using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.ldap
{
    public abstract class LdapModel
    {
        protected static readonly string[] LoadProperties = {
            LdapProperties.CommonName,
            LdapProperties.ObjectClass
        };

        protected string ObjectClass;
        protected virtual string DefaultObjectClass => string.Empty;
        protected virtual Dictionary<PropertyInfo, LdapAttr> Properties => new Dictionary<PropertyInfo, LdapAttr>();

        protected LdapEntry Entry { get; private set; }

        /// <summary>
        ///     Id of the object (cn=*).
        /// </summary>
        [JsonProperty("Id")]
        [Required]
        public string Id { get; set; }

        /// <summary>
        ///     Domainname of the object.
        /// </summary>
        [JsonProperty("Dn")]
        [Required]
        public string Dn { get; set; }

        /// <summary>
        ///     Converts a ldap entry to the ldap model object.
        /// </summary>
        /// <param name="entry">Entry to convert.</param>
        public virtual void ProvideEntry(LdapEntry entry) {
            this.ObjectClass = entry.GetAttribute(LdapProperties.ObjectClass);
            this.Id = entry.GetAttribute(LdapProperties.CommonName);
            this.Dn = entry.DN;
            this.Entry = entry;

            // load properties with reflection
            foreach (KeyValuePair<PropertyInfo, LdapAttr> kv in this.Properties) {
                object value;
                switch (Type.GetTypeCode(kv.Value.Type)) {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        value = entry.GetIntAttribute(kv.Value);
                        break;
                    case TypeCode.Boolean:
                        value = entry.GetBoolAttribute(kv.Value);
                        break;
                    case TypeCode.DateTime:
                        value = entry.GetDateTimeAttribute(kv.Value);
                        break;
                    case TypeCode.Object:
                        if (kv.Value.Type == typeof(List<string>)) {
                            value = entry.GetStringListAttribute(kv.Value);
                            break;
                        }

                        value = null;
                        break;
                    default:
                        value = entry.GetAttribute(kv.Value);
                        break;
                }

                kv.Key.SetValue(this, value);
            }
        }

        /// <summary>
        ///     Convertas a ldap model to the corresponding ldap attribute set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public virtual LdapAttributeSet GetAttributeSet(LdapAttributeSet set = null) {
            set = set ?? new LdapAttributeSet();
            set.Add(LdapProperties.ObjectClass, this.ObjectClass ?? this.DefaultObjectClass)
                .Add(LdapProperties.CommonName, this.Id);

            foreach (KeyValuePair<PropertyInfo, LdapAttr> kvp in this.Properties)
                set.Add(kvp.Value, kvp.Key.GetValue(this));

            return set;
        }

        protected virtual List<LdapModification> GetModifcationsList(List<LdapModification> list = null) {
            List<LdapModification> modifications = new List<LdapModification>();
            return modifications;
        }

        public LdapModification[] GetModifications() {
            return this.GetModifcationsList().ToArray();
        }

        public LdapEntry ToEntry() {
            return new LdapEntry(this.Dn, this.GetAttributeSet());
        }
    }
}