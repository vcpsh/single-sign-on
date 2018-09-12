using System;
using System.Collections.Generic;
using Novell.Directory.Ldap;

namespace sh.vcp.ldap
{
    /// <summary>
    /// Configures the backing ldap property of an object in a model.
    /// </summary>
    /// <remarks>
    /// Currently supported are: int(?), bool, DateTime(?), string, List(string)
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class LdapAttr : Attribute 
    {
        public LdapAttr(string ldapName, bool optional = false) {
            this.Optional = optional;
            this.LdapName = ldapName;
            this.Type = typeof(string);
        }

        public LdapAttr(string ldapName, Type type, bool optional = false) {
            this.Optional = optional;
            this.LdapName = ldapName;
            this.Type = type;
        }

        internal bool Optional { get; }
        internal string LdapName { get; }
        internal Type Type { get; }

        public LdapAttribute CreateLdapAttribute(object value)
        {
            if (value == null && !this.Optional)
                throw new ArgumentNullException(nameof(value), $"Attribute \"{this.LdapName}\"");

            LdapAttribute ldapAttr = null;
            switch (Type.GetTypeCode(this.Type)) {
                case TypeCode.Boolean:
                    ldapAttr = new LdapAttribute(this.LdapName, (bool) value ? "TRUE" : "FALSE");
                    break;
                case TypeCode.DateTime:
                    ldapAttr = new LdapAttribute(this.LdapName,
                        ((DateTime) value).ToString(LdapConstants.DateFormat));
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    ldapAttr = new LdapAttribute(this.LdapName, ((int) value).ToString());
                    break;
                case TypeCode.Object:
                    if (this.Type == typeof(List<string>) && value is List<string> list)
                        if (list.Count > 0)
                            ldapAttr = new LdapAttribute(this.LdapName, list.ToArray());

                    break;
                case TypeCode.String:
                default:
                    ldapAttr = new LdapAttribute(this.LdapName, (string) value);
                    break;
            }

            return ldapAttr;
        }
    }
}