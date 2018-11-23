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

        public static IEnumerable<Change> ToChangesAdd(this LdapAttributeSet set, string dn, string changedBy) {
            List<Change> changes = new List<Change>();
            var objectClass = set.getAttribute(LdapProperties.ObjectClass).StringValue;
            var guid = Guid.NewGuid();
            foreach (LdapAttribute attr in set) {
                changes.AddRange(attr.StringValueArray.Select(val => new Change
                {
                    Dn = dn,
                    Type = attr.Name == LdapProperties.CommonName
                        ? Change.TypeEnum.Created
                        : Change.TypeEnum.CreatedAttribute,
                    ObjectClass = objectClass,
                    Property = attr.Name,
                    NewValue = attr.Name == LdapProperties.UserPassword ? "****" : attr.StringValue,
                    ChangeContext = guid,
                    ChangedBy = changedBy,
                }));
            }

            return changes;
        }

        public static IEnumerable<Change> ToChangesModify(this LdapModification[] modifications, string dn, string changedBy,
            List<string> objectClasses) {
            List<Change> changes = new List<Change>();
            var guid = Guid.NewGuid();
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
                changes.AddRange(modification.Attribute.StringValueArray.Select(val => new Change
                {
                    Dn = dn,
                    Type = change,
                    ObjectClass = objectClasses.Aggregate("", (str, oc) => str == "" ? oc : $"{str};{oc}"),
                    Property = modification.Attribute.Name,
                    NewValue = modification.Attribute.Name == LdapProperties.UserPassword ? "****" : val,
                    ChangedBy = changedBy,
                    ChangeContext = guid,
                }));
            }

            return changes;
        }
    }
}