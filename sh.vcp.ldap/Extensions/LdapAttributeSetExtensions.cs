using System;
using System.Collections.Generic;
using System.Linq;
using Novell.Directory.Ldap;

namespace sh.vcp.ldap.Extensions
{
    public static class LdapAttributeSetExtensions
    {
        public static LdapAttributeSet Add(this LdapAttributeSet set, string attributeName, string attributeValue)
        {
            if (attributeValue == null)
            {
                throw new ArgumentNullException(nameof(attributeValue), $"Attribute \"{attributeName}\"");
            }

            set.Add(new LdapAttribute(attributeName, attributeValue));
            return set;
        }

        public static LdapAttributeSet Add(this LdapAttributeSet set, string attributeName, int attributeValue)
        {
            set.Add(attributeName, attributeValue.ToString());
            return set;
        }

        public static LdapAttributeSet Add(this LdapAttributeSet set, string attributeName,
            IEnumerable<string> attributeList)
        {
            set.Add(new LdapAttribute(attributeName, attributeList.ToArray()));
            return set;
        }

        public static LdapAttributeSet AddOptional(this LdapAttributeSet set, string attributeName,
            string attributeValue)
        {
            if (!string.IsNullOrEmpty(attributeValue))
            {
                set.Add(attributeName, attributeValue);
            }

            return set;
        }

        public static LdapAttributeSet AddOptional(this LdapAttributeSet set, string attributeName, bool attributeValue)
        {
            return set.AddOptional(attributeName, attributeValue.ToString());
        }

        public static LdapAttributeSet AddOptional<TList>(this LdapAttributeSet set, string attributeName,
            TList attributeList) where TList : IEnumerable<string>, ICollection<string>
        {
            if (attributeList != null && attributeList.Count > 0)
            {
                set.Add(attributeName, attributeList);
            }

            return set;
        }
    }
}