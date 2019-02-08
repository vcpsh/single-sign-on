namespace sh.vcp.ldap
{
    public static class LdapProperties
    {
        public const string ObjectClass = "objectClass";
        public const string CommonName = "cn";
        public const string AccessionDate = "x-sh-vcp-accession-date";
        public const string AdminVerified = "x-sh-vcp-admin-verified";
        public const string DateOfBirth = "x-sh-vcp-date-of-birth";
        public const string Email = "email";
        public const string OfficialMail = "x-sh-vcp-email";
        public const string EmailVerified = "x-sh-vcp-email-verified";
        public const string Gender = "x-sh-vcp-gender";
        public const string VcpId = "x-sh-vcp-id";
        public const string DepartmentId = "departmentNumber";
        public const string Uid = "uid";
        public const string FirstName = "givenName";
        public const string LastName = "sn";
        public const string Member = "memberUid";
        public const string DisplayName = "displayName";
        public const string Active = "x-sh-vcp-active";
        public const string VoteStartDate = "x-sh-vcp-vote-start-date";
        public const string VoteStartEvent = "x-sh-vcp-vote-start-event";
        public const string VoteEndDate = "x-sh-vcp-vote-end-date";
        public const string VoteEndEvent = "x-sh-vcp-vote-end-event";
        public const string UserPassword = "userPassword";
    }

    public static class LdapObjectTypes
    {
        public const string User = "x-sh-vcp-user";
        public const string Member = "x-sh-vcp-member";
        public const string Tribe = "x-sh-vcp-tribe";
        public const string TribeGs = "x-sh-vcp-tribe-gs";
        public const string TribeSl = "x-sh-vcp-tribe-sl";
        public const string TribeLr = "x-sh-vcp-tribe-lr";
        public const string TribeLv = "x-sh-vcp-tribe-lv";
        public const string TribeGroup = "x-sh-vcp-tribe-group";
        public const string WaitingUser = "x-sh-vcp-waiting-user";
        public const string Group = "x-sh-vcp-group";
        public const string Division = "x-sh-vcp-division";
        public const string OrgUnit = "x-sh-vcp-org-unit";
        public const string VotedGroup = "x-sh-vcp-voted-group";
        public const string VotedEntry = "x-sh-vcp-vote-entry";
    }

    public static class LdapConstants
    {
        public const string DateFormat = "yyyyMMddHHmmssZ";
    }
}