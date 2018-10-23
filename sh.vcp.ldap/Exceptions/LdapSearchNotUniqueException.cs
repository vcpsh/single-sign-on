using System;

namespace sh.vcp.ldap.Exceptions
{
    public class LdapSearchNotUniqueException : Exception
    {
        public LdapSearchNotUniqueException(string filter, int count) : base(
            $"Expected 1 element for filter \"{filter}\" but got \"{count}\"") { }
    }
}