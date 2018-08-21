using System;

namespace sh.vcp.ldap.Exceptions
{
    public class LdapDnInvalidException : Exception
    {
        public LdapDnInvalidException(string dn) : base($"Got an invalid dn \"{dn}\".") {
        }
    }
}