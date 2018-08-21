using System;
using System.Collections.Generic;
using System.Reflection;

namespace sh.vcp.ldap
{
    public static class LdapAttrHelper
    {
        public static Dictionary<PropertyInfo, LdapAttr> GetLdapAttrs(Type ldapModel) {
            Dictionary<PropertyInfo, LdapAttr> dict = new Dictionary<PropertyInfo, LdapAttr>();
            foreach (var propertyInfo in ldapModel.GetProperties())
                if (propertyInfo.GetCustomAttribute(typeof(LdapAttr), true) is LdapAttr attr)
                    dict.Add(propertyInfo, attr);

            return dict;
        }
    }
}