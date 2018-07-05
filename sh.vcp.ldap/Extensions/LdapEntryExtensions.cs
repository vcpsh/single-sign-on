using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Novell.Directory.Ldap;
using sh.vcp.ldap.Exceptions;

namespace sh.vcp.ldap.Extensions
{
    public static class LdapEntryExtensions
    {
        public static string GetAttribute(this LdapEntry entry, string attributeName)
        {
            return entry.getAttribute(attributeName)?.StringValue ?? throw new LdapMandatoryAttributeMissingException(attributeName, entry.DN);
        }

        public static int GetAttributeInt(this LdapEntry entry, string attributeName)
        {
            return int.Parse(entry.GetAttribute(attributeName));
        }

        public static string GetOptionalAttribute(this LdapEntry entry, string attributeName)
        {
            return entry.getAttribute(attributeName)?.StringValue;
        }

        public static List<string> GetOptionalListAttribute(this LdapEntry entry, string attributeName)
        {
            return entry.getAttribute(attributeName)?.StringValueArray.ToList() ?? new List<string>();
        }
    }
}