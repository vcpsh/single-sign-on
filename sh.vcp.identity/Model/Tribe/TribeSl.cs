﻿using Novell.Directory.Ldap;
using sh.vcp.identity.Models;
using sh.vcp.ldap;

namespace sh.vcp.identity.Model.Tribe
{
    public class TribeSl: LdapGroup
    {
        protected override string __defaultObjectClass => LdapObjectTypes.TribeSl;
        
        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
        }
    }
}