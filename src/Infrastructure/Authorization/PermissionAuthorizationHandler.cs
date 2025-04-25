using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Authorization;
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var hasClaim = context.User.Claims.Any(c =>
            c.Type == "Permission" && c.Value == requirement.Permission);

        if (hasClaim)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
