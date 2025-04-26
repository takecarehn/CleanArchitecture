using System.Security.Claims;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Authorization;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Endpoints;

[Authorize]
public class UserPermissions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/claims").WithTags("Claims"); ;

        group.MapPost("/roles/{roleId}/add", AddClaimsToRoleAsync)
             .RequirePermission(ClaimValues.PermissionRoleAddClaims);

        group.MapPost("/users/{userId:guid}/add", AddClaimsToUserAsync)
             .RequirePermission(ClaimValues.PermissionUserAddClaims);

        group.MapGet("/users/{userId:guid}/all", GetAllClaimsOfUserAsync)
             .RequirePermission(ClaimValues.PermissionUserViewClaims);

        group.MapPut("/roles/{roleId}/update", UpdateClaimsOfRoleAsync)
             .RequirePermission(ClaimValues.PermissionRoleUpdateClaims);

        group.MapPut("/users/{userId:guid}/update", UpdateClaimsOfUserAsync)
             .RequirePermission(ClaimValues.PermissionUserUpdateClaims);

        group.MapGet("/all", GetAllDefinedClaimsAsync)
             .RequirePermission(ClaimValues.PermissionAllClaims);
    }

    private static async Task<Result> AddClaimsToRoleAsync(
    [FromServices] RoleManager<IdentityRole> roleManager,
    string roleId,
    [FromBody] ClaimsRequest request)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null) return Result.Failure("Role not found!");

        foreach (var claimValue in request.Claims)
        {
            await roleManager.AddClaimAsync(role, new Claim(request.ClaimType, claimValue));
        }

        return Result.Success("Added claims to role");
    }

    private static async Task<Result> AddClaimsToUserAsync(
    [FromServices] UserManager<ApplicationUser> userManager,
    string userId,
    [FromBody] ClaimsRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure("User not found!");

        foreach (var claimValue in request.Claims)
        {
            await userManager.AddClaimAsync(user, new Claim(request.ClaimType, claimValue));
        }

        return Result.Success("Added claims to user");
    }

    private static async Task<Result> GetAllClaimsOfUserAsync(
        [FromServices] UserManager<ApplicationUser> userManager,
        [FromServices] IIdentityService identityService,
        string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure("User not found!");
        var userPermissions = await identityService.GetUserPermissionsAsync(userId);

        return Result.Success(userPermissions);
    }

    private static async Task<Result> UpdateClaimsOfRoleAsync(
    [FromServices] RoleManager<IdentityRole> roleManager,
    string roleId,
    [FromBody] ClaimsRequest request)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null) return Result.Failure("Role not found!");

        var currentClaims = await roleManager.GetClaimsAsync(role);

        foreach (var claim in currentClaims)
        {
            await roleManager.RemoveClaimAsync(role, claim);
        }

        foreach (var claimValue in request.Claims)
        {
            await roleManager.AddClaimAsync(role, new Claim(request.ClaimType, claimValue));
        }

        return Result.Success("Updated claims for role");
    }

    private static async Task<Result> UpdateClaimsOfUserAsync(
    [FromServices] UserManager<ApplicationUser> userManager,
    string userId,
    [FromBody] ClaimsRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure("User not found!");

        var currentClaims = await userManager.GetClaimsAsync(user);

        foreach (var claim in currentClaims)
        {
            await userManager.RemoveClaimAsync(user, claim);
        }

        foreach (var claimValue in request.Claims)
        {
            await userManager.AddClaimAsync(user, new Claim(request.ClaimType, claimValue));
        }

        return Result.Success("Updated claims for user");
    }

    private static Task<Result> GetAllDefinedClaimsAsync()
    {
        var claimValues = typeof(ClaimValues)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.GetValue(null)?.ToString())
            .Where(value => value != null && value.StartsWith("Permission."))
            .GroupBy(value =>
            {
                if (value == null) return string.Empty; // Handle null case explicitly
                var lastDot = value.LastIndexOf('.');
                return lastDot != -1 ? value.Substring(0, lastDot) : value;
            })
            .ToDictionary(group => group.Key, group => group.ToList());

        return Task.FromResult(Result.Success(claimValues));
    }
}

public record ClaimsRequest(string ClaimType, List<string> Claims);
