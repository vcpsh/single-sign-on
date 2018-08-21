using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Novell.Directory.Ldap;
using sh.vcp.ldap.Exceptions;

namespace sh.vcp.ldap.Extensions
{
    public static class LdapEntryExtensions
    {
        public static string GetAttribute(this LdapEntry entry, string attributeName, bool optional = true) {
            var val = entry.getAttribute(attributeName)?.StringValue;
            if (!optional && val == null) throw new LdapMandatoryAttributeMissingException(attributeName, entry.DN);

            return val;
        }

        public static string GetAttribute(this LdapEntry entry, LdapAttr attr) {
            return entry.GetAttribute(attr.LdapName, attr.Optional);
        }

        public static bool GetBoolAttribute(this LdapEntry entry, LdapAttr attr) {
            return entry.GetAttribute(attr) == "TRUE";
        }

        public static int GetIntAttribute(this LdapEntry entry, LdapAttr attr) {
            return int.Parse(entry.GetAttribute(attr));
        }

        public static DateTime? GetDateTimeAttribute(this LdapEntry entry, LdapAttr attr) {
            var val = entry.GetAttribute(attr);
            if (val == null && attr.Optional) return null;

            return DateTime.ParseExact(entry.GetAttribute(attr), LdapConstants.DateFormat,
                CultureInfo.InvariantCulture);
        }

        public static List<string> GetStringListAttribute(this LdapEntry entry, LdapAttr attr) {
            List<string> list = entry.getAttribute(attr.LdapName)?.StringValueArray.ToList();
            if (list == null && !attr.Optional)
                throw new LdapMandatoryAttributeMissingException(attr.LdapName, entry.DN);

            return list ?? new List<string>();
        }

        [Obsolete]
        public static bool GetOptionalBoolAttribute(this LdapEntry entry, string attributeName) {
            return entry.GetOptionalAttribute(attributeName) == "TRUE";
        }

        [Obsolete]
        public static string GetOptionalAttribute(this LdapEntry entry, string attributeName) {
            return entry.getAttribute(attributeName)?.StringValue;
        }

        [Obsolete]
        public static List<string> GetOptionalListAttribute(this LdapEntry entry, string attributeName) {
            return entry.getAttribute(attributeName)?.StringValueArray.ToList() ?? new List<string>();
        }
    }
}