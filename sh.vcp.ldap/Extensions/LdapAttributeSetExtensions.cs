using System;
using System.Collections.Generic;
using System.Linq;
using Novell.Directory.Ldap;
using sh.vcp.ldap.ChangeTracking;

namespace sh.vcp.ldap.Extensions
{
    public static class LdapAttributeSetExtensions
    {
        public static LdapAttributeSet Add(this LdapAttributeSet set, LdapAttr attr, object value) {
            var ldapAttribute = attr.CreateLdapAttribute(value);

            if (value == null) {
                return set;
            }

            if (ldapAttribute != null) set.Add(ldapAttribute);
            return set;
        }

        public static IEnumerable<Change> ToChangesAdd(this LdapAttributeSet set, string dn) {
            List<Change> changes = new List<Change>();
            string objectClass = set.getAttribute(LdapProperties.ObjectClass).StringValue;
            foreach (LdapAttribute attr in set) {
                changes.Add(new Change {
                    Dn = dn,
                    Type = attr.Name == LdapProperties.CommonName
                        ? Change.TypeEnum.Created
                        : Change.TypeEnum.CreatedAttribute,
                    ObjectClass = objectClass,
                    Property = attr.Name,
                    NewValue = attr.StringValue,
                });
            }

            return changes;
        }

        public static IEnumerable<Change> ToChangesModify(this LdapModification[] modifications, string dn,
            List<string> objectClasses) {
            List<Change> changes = new List<Change>();
            foreach (var modification in modifications) {
                Change.TypeEnum change;
                switch (modification.Op) {
                    case LdapModification.ADD:
                        change = Change.TypeEnum.CreatedAttribute;
                        break;
                    case LdapModification.DELETE:
                        change = Change.TypeEnum.RemovedAttribute;
                        break;
                    case LdapModification.REPLACE:
                        change = Change.TypeEnum.Modified;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                changes.Add(new Change {
                    Dn = dn,
                    Type = change,
                    ObjectClass = objectClasses.Aggregate("", (str, oc) => str == "" ? oc : $"{str};{oc}"),
                    Property = modification.Attribute.Name,
                    NewValue = modification.Attribute.StringValue
                });
            }

            return changes;
        }
    }
}