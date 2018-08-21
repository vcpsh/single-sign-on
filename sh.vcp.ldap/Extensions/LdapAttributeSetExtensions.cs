using System;
using System.Collections.Generic;
using System.Linq;
using Novell.Directory.Ldap;

namespace sh.vcp.ldap.Extensions
{
    public static class LdapAttributeSetExtensions
    {
        public static LdapAttributeSet Add(this LdapAttributeSet set, LdapAttr attr, object value) {
            if (value == null && !attr.Optional)
                throw new ArgumentNullException(nameof(value), $"Attribute \"{attr.LdapName}\"");

            if (value == null) {
                return set;
            }

            LdapAttribute ldapAttr = null;
            switch (Type.GetTypeCode(attr.Type)) {
                case TypeCode.Boolean:
                    ldapAttr = new LdapAttribute(attr.LdapName, (bool) value ? "TRUE" : "FALSE");
                    break;
                case TypeCode.DateTime:
                    ldapAttr = new LdapAttribute(attr.LdapName,
                        ((DateTime) value).ToString(LdapConstants.DateFormat));
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    ldapAttr = new LdapAttribute(attr.LdapName, ((int) value).ToString());
                    break;
                case TypeCode.Object:
                    if (attr.Type == typeof(List<string>) && value is List<string> list)
                        if (list.Count > 0)
                            ldapAttr = new LdapAttribute(attr.LdapName, list.ToArray());

                    break;
                case TypeCode.String:
                default:
                    ldapAttr = new LdapAttribute(attr.LdapName, (string) value);
                    break;
            }

            if (ldapAttr != null) set.Add(ldapAttr);

            return set;
        }

        [Obsolete]
        public static LdapAttributeSet Add(this LdapAttributeSet set, string attributeName, string attributeValue) {
            if (attributeValue == null)
                throw new ArgumentNullException(nameof(attributeValue), $"Attribute \"{attributeName}\"");

            set.Add(new LdapAttribute(attributeName, attributeValue));
            return set;
        }

        [Obsolete]
        public static LdapAttributeSet Add(this LdapAttributeSet set, string attributeName, int attributeValue) {
            set.Add(attributeName, attributeValue.ToString());
            return set;
        }

        [Obsolete]
        public static LdapAttributeSet Add(this LdapAttributeSet set, string attributeName,
            IEnumerable<string> attributeList) {
            set.Add(new LdapAttribute(attributeName, attributeList.ToArray()));
            return set;
        }

        [Obsolete]
        public static LdapAttributeSet AddOptional(this LdapAttributeSet set, string attributeName,
            string attributeValue) {
            if (!string.IsNullOrEmpty(attributeValue)) set.Add(attributeName, attributeValue);

            return set;
        }

        [Obsolete]
        public static LdapAttributeSet
            AddOptional(this LdapAttributeSet set, string attributeName, bool attributeValue) {
            return set.AddOptional(attributeName, attributeValue.ToString());
        }

        [Obsolete]
        public static LdapAttributeSet AddOptional<TList>(this LdapAttributeSet set, string attributeName,
            TList attributeList) where TList : IEnumerable<string>, ICollection<string> {
            if (attributeList != null && attributeList.Count > 0) set.Add(attributeName, attributeList);

            return set;
        }
    }
}