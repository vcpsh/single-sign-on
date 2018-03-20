using System;

namespace sh.vcp.ldap.Exceptions
{
    public class LdapMandatoryAttributeMissingException: Exception
    {
        
        public LdapMandatoryAttributeMissingException(string attributeName, string dn): base($"Expected mandatory attribute \"{attributeName}\" for element \"{dn}\" but none was found.")
        {
        }
    }
}