namespace WLS.Authentication.Constants
{
    public static class JwtSchemes
    {
        public const string Default = "WLSAuthScheme";
    }
    public static class JwtClaimIdentifiers
    {
        public const string
            Rol = "rol",
            Id = "id",
            SiteType = "site",
            UserName = "user_name",
            UserType = "user_type",
            RoleAccessRefIDs = "rar";
    }

}