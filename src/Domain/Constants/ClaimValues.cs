namespace CleanArchitecture.Domain.Constants;

public abstract class ClaimValues
{
    public const string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
    public const string NameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

    // User claims
    public const string Permission = "permissions";

    public const string PermissionUserAdd = "Permission.User.Add";
    public const string PermissionUserUpdate = "Permission.User.Update";
    public const string PermissionUserDelete = "Permission.User.Delete";
    public const string PermissionUserGetUserName = "Permission.User.GetUserName";
    public const string PermissionUserLock = "Permission.User.Lock";
    public const string PermissionUserUnlock = "Permission.User.Unlock";
    public const string PermissionUserChangePass = "Permission.User.ChangePass";
    public const string PermissionUserResetPass = "Permission.User.ResetPass";

    public const string PermissionAllClaims = "Permission.RoleClaim.AllClaims";
    public const string PermissionRoleAddClaims = "Permission.RoleClaim.AddClaims";
    public const string PermissionRoleUpdateClaims = "Permission.RoleClaim.UpdateClaims";
    public const string PermissionUserAddClaims = "Permission.UserClaim.AddClaims";
    public const string PermissionUserViewClaims = "Permission.UserClaim.ViewClaims";
    public const string PermissionUserUpdateClaims = "Permission.UserClaim.UpdateClaims";

    public const string PermissionRoleAdd = "Permission.Role.Add";
    public const string PermissionRoleUpdate = "Permission.Role.Update";
    public const string PermissionRoleDelete = "Permission.Role.Delete";
    public const string PermissionRoleAll = "Permission.Role.GetAll";

    public const string PermissionUpload = "Permission.RoleClaim.Upload";
}
