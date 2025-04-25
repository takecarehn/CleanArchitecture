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

    public const string PermissionTodoListGetTodo = "Permission.TodoList.GetTodo";
}
