using System;

namespace sh.vcp.ldap
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LdapAttr: Attribute
    {
        internal bool Optional { get; }
        internal string LdapName { get; }
        internal Type Type { get; }

        public LdapAttr(string ldapName, bool optional = false)
        {
            this.Optional = optional;
            this.LdapName = ldapName;
            this.Type = typeof(string);
        }
        public LdapAttr(string ldapName, Type type, bool optional = false)
        {
            this.Optional = optional;
            this.LdapName = ldapName;
            this.Type = type;
        }
    }
}