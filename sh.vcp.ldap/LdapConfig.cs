using Microsoft.Extensions.Configuration;

namespace sh.vcp.ldap
{
    public class LdapConfig
    {
        /// <summary>
        ///     port of the ldap database
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }

        /// <summary>
        ///     hostname of the ldap database
        /// </summary>
        /// <value>The hostname.</value>
        public string Hostname { get; set; }

        /// <summary>
        ///     DN of the admin user
        /// </summary>
        /// <value>The admin user dn.</value>
        public string AdminUserDn { get; set; }

        /// <summary>
        ///     Password of the admin user
        /// </summary>
        /// <value>The admin user password.</value>
        public string AdminUserPassword { get; set; }

        /// <summary>
        ///     Base DN of the ldap Database
        /// </summary>
        /// <value>The base dn</value>
        public string BaseDn { get; set; }

        /// <summary>
        ///     Relative to the BaseDN
        /// </summary>
        public string RelativeMemberDn { get; set; }

        /// <summary>
        ///     Dn of all the members
        /// </summary>
        public string MemberDn => $"{this.RelativeMemberDn},{this.BaseDn}";

        /// <summary>
        ///     Relative to the BaseDn
        /// </summary>
        public string RelativeGroupDn { get; set; }
        
        /// <summary>
        ///     Dn of all apis
        /// </summary>
        public string GroupDn => $"{this.RelativeGroupDn},{this.BaseDn}";
        
        /// <summary>
        /// Log the changes to a change database
        /// </summary>
        public bool LogChanges { get; set; }

        public IConfigurationSection AuthorizationConfigurationSection { get; set; }
    }
}