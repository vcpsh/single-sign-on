using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using sh.vcp.ldap;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.identity.Model
{
    public class LdapMember : LdapUser
    {
        protected override string __defaultObjectClass => LdapObjectTypes.Member;

        public new static readonly string[] LoadProperties = new string[]
        {
            LdapProperties.FirstName,
            LdapProperties.LastName,
            LdapProperties.DateOfBirth,
            LdapProperties.AccessionDate,
            LdapProperties.Gender,
        }.Concat(LdapModel.LoadProperties).ToArray();

        [JsonProperty("FirstName")] public string FirstName { get; set; }

        [JsonProperty("LastName")] public string LastName { get; set; }

        [JsonProperty("DateOfBirth")] public DateTime DateOfBirth { get; set; }
        
        [JsonProperty("AccessionDate")] public DateTime AccessionDate { get; set; }
        
        [JsonProperty("Gender")] public string Gender { get; set; }

        public override void ProvideEntry(LdapEntry entry)
        {
            base.ProvideEntry(entry);
            this.FirstName = entry.GetAttribute(LdapProperties.FirstName);
            this.LastName = entry.GetAttribute(LdapProperties.LastName);
            this.DateOfBirth = DateTime.ParseExact(entry.GetAttribute(LdapProperties.DateOfBirth), LdapConstants.DateFormat, CultureInfo.InvariantCulture);
            this.AccessionDate = DateTime.ParseExact(entry.GetAttribute(LdapProperties.AccessionDate), LdapConstants.DateFormat, CultureInfo.InvariantCulture);
            this.Gender = entry.GetAttribute(LdapProperties.Gender);
        }

        public override LdapAttributeSet GetAttributeSet(LdapAttributeSet set = null)
        {
            return base.GetAttributeSet(set)
                .Add(LdapProperties.FirstName, this.FirstName)
                .Add(LdapProperties.LastName, this.LastName)
                .Add(LdapProperties.DateOfBirth, this.DateOfBirth.ToString(LdapConstants.DateFormat))
                .Add(LdapProperties.AccessionDate, this.AccessionDate.ToString(LdapConstants.DateFormat))
                .Add(LdapProperties.Gender, this.Gender);
        }
    }
}