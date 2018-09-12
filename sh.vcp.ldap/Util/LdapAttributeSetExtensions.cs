using System;
using System.Collections.Generic;
using System.Linq;
using Novell.Directory.Ldap;

namespace sh.vcp.ldap.Util
{
    public static class LdapAttributeSetExtensions
    {
        public static LdapAttributeSet Add(this LdapAttributeSet set, LdapAttr attr, object value)
        {
            var ldapAttribute = attr.CreateLdapAttribute(value);
            
            if (value == null) {
                return set;
            }
            if (ldapAttribute != null) set.Add(ldapAttribute);
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