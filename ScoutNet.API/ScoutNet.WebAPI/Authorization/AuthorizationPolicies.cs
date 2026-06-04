namespace ScoutNet.WebAPI.Authorization;

public static class AuthorizationPolicies
{
    public const string ScoutOrAdmin = "ScoutOrAdmin";
    public const string AdminOnly = "AdminOnly";
}

public static class AppRoles
{
    public const string Scout = "Scout";
    public const string Admin = "Admin";
}
