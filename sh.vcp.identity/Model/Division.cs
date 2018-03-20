using System;
using System.Linq;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Models
{
    public class Division: LdapGroup
    {
        public new static readonly string[] LoadProperties = new[]
        {
            LdapProperties.DepartmentId
        }.Concat(LdapGroup.LoadProperties).ToArray();
        
        public int DepartmentId { get; set; }

        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
            this.DepartmentId = int.Parse(entry.GetAttribute(LdapProperties.DepartmentId));
        }

        public override LdapAttributeSet GetAttributeSet(LdapAttributeSet set = null)
        {
            return base.GetAttributeSet(set)
                .Add(LdapProperties.DepartmentId, this.DepartmentId);
        }
    }
}