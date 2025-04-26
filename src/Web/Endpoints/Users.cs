using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Endpoints;

[Authorize]
public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users"); ;
        group.MapGet("/{userId:guid}/user-name", GetUserNameAsync)
             .RequirePermission(ClaimValues.PermissionUserGetUserName);
        group.MapPost("/create", CreateUserAsync)
             .RequirePermission(ClaimValues.PermissionUserAdd);
        group.MapGet("/{userId:guid}/role/{role::guid}", IsInRoleAsync);
        group.MapPost("/{userId:guid}/authorize/{policyName}", AuthorizeAsync);
        group.MapDelete("/{userId:guid}", DeleteUserAsync)
             .RequirePermission(ClaimValues.PermissionUserDelete);
        group.MapPost("/{userId:guid}/lock", LockUserAsync)
             .RequirePermission(ClaimValues.PermissionUserLock);
        group.MapPost("/{userId:guid}/unlock", UnlockUserAsync)
             .RequirePermission(ClaimValues.PermissionUserUnlock);
    }

    private static async Task<Result> GetUserNameAsync(
        [FromServices] IIdentityService identityService,
        string userId)
    {
        var userName = await identityService.GetUserNameAsync(userId);
        return userName != null ? Result.Success(userName) : Result.Failure("User not found");
    }

    private static async Task<Result> CreateUserAsync(
        [FromServices] IIdentityService identityService,
        [FromBody] CreateUserRequest request)
    {
        var (result, userId) = await identityService.CreateUserAsync(request.UserName, request.Password);
        return result.Succeeded ? Result.Success(new { UserId = userId }) : Result.Failure(result.Errors);
    }

    private static async Task<Result> IsInRoleAsync(
        [FromServices] IIdentityService identityService,
        string userId,
        string role)
    {
        var isInRole = await identityService.IsInRoleAsync(userId, role);
        return Result.Success(new { IsInRole = isInRole });
    }

    private static async Task<Result> AuthorizeAsync(
        [FromServices] IIdentityService identityService,
        string userId,
        string policyName)
    {
        var isAuthorized = await identityService.AuthorizeAsync(userId, policyName);
        return Result.Success(new { IsAuthorized = isAuthorized });
    }

    private static async Task<Result> DeleteUserAsync(
        [FromServices] IIdentityService identityService,
        string userId)
    {
        var result = await identityService.DeleteUserAsync(userId);
        return result.Succeeded ? Result.Success("User deleted successfully") : Result.Failure(result.Errors);
    }

    private static async Task<Result> LockUserAsync(
    [FromServices] IIdentityService identityService,
    string userId)
    {
        return await identityService.LockUserAsync(userId);
    }

    private static async Task<Result> UnlockUserAsync(
        [FromServices] IIdentityService identityService,
        string userId)
    {
        return await identityService.UnlockUserAsync(userId);
    }
}

public record CreateUserRequest(string UserName, string Password);
