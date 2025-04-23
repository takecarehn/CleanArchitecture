using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/users");
        group.RequireAuthorization();
        group.MapGet("/{userId}/username", GetUserNameAsync);
        group.MapPost("/create", CreateUserAsync);
        group.MapGet("/{userId}/role/{role}", IsInRoleAsync);
        group.MapPost("/{userId}/authorize/{policyName}", AuthorizeAsync);
        group.MapDelete("/{userId}", DeleteUserAsync);
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
}

public record CreateUserRequest(string UserName, string Password);
