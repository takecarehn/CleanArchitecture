using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/{userId}/username", GetUserNameAsync);
        group.MapPost("/create", CreateUserAsync);
        group.MapGet("/{userId}/role/{role}", IsInRoleAsync);
        group.MapPost("/{userId}/authorize/{policyName}", AuthorizeAsync);
        group.MapDelete("/{userId}", DeleteUserAsync);
    }

    private static async Task<IResult> GetUserNameAsync(
        [FromServices] IIdentityService identityService,
        string userId)
    {
        var userName = await identityService.GetUserNameAsync(userId);
        return userName != null ? Results.Ok(userName) : Results.NotFound("User not found");
    }

    private static async Task<IResult> CreateUserAsync(
        [FromServices] IIdentityService identityService,
        [FromBody] CreateUserRequest request)
    {
        var (result, userId) = await identityService.CreateUserAsync(request.UserName, request.Password);
        return result.Succeeded ? Results.Ok(new { UserId = userId }) : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> IsInRoleAsync(
        [FromServices] IIdentityService identityService,
        string userId,
        string role)
    {
        var isInRole = await identityService.IsInRoleAsync(userId, role);
        return Results.Ok(new { IsInRole = isInRole });
    }

    private static async Task<IResult> AuthorizeAsync(
        [FromServices] IIdentityService identityService,
        string userId,
        string policyName)
    {
        var isAuthorized = await identityService.AuthorizeAsync(userId, policyName);
        return Results.Ok(new { IsAuthorized = isAuthorized });
    }

    private static async Task<IResult> DeleteUserAsync(
        [FromServices] IIdentityService identityService,
        string userId)
    {
        var result = await identityService.DeleteUserAsync(userId);
        return result.Succeeded ? Results.Ok("User deleted successfully") : Results.BadRequest(result.Errors);
    }
}

public record CreateUserRequest(string UserName, string Password);
