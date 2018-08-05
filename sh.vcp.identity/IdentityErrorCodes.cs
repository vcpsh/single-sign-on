namespace sh.vcp.identity
{
    internal static class IdentityErrorCodes
    {
        internal const string UserStoreFindById = nameof(IdentityErrorCodes.UserStoreFindById);
        internal const string UserStoreFindByName = nameof(IdentityErrorCodes.UserStoreFindByName);
        internal const string UserStoreFindByEmail = nameof(IdentityErrorCodes.UserStoreFindByEmail);
        internal const string UserStoreGetClaims = nameof(IdentityErrorCodes.UserStoreGetClaims);
        internal const string SetUserPasswordAsync = nameof(IdentityErrorCodes.SetUserPasswordAsync);
    }
}