using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Identity.Roles;
using CleanArchitecture.Application.Identity.Roles.Commands;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Endpoints;

public class RoleEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/roles").WithTags("Roles");

        group.MapPost("/", CreateRoleAsync).RequirePermission(ClaimValues.PermissionRoleAdd);
        group.MapPut("/{roleId}", UpdateRoleAsync).RequirePermission(ClaimValues.PermissionRoleUpdate);
        group.MapDelete("/{roleId}", DeleteRoleAsync).RequirePermission(ClaimValues.PermissionRoleDelete);
        group.MapGet("/", GetRolesAsync).RequirePermission(ClaimValues.PermissionRoleAll);
    }

    private static async Task<Result> CreateRoleAsync(
        [FromServices] RoleManager<IdentityRole> roleManager,
        [FromBody] CreateOrUpdateRoleRequest request)
    {
        if (await roleManager.RoleExistsAsync(request.Name))
        {
            return Result.Failure("Role already exists.");
        }

        var result = await roleManager.CreateAsync(new IdentityRole(request.Name));

        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    private static async Task<Result> UpdateRoleAsync(
        [FromServices] RoleManager<IdentityRole> roleManager,
        string roleId,
        [FromBody] CreateOrUpdateRoleRequest request)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return Result.Failure("Role not found.");
        }

        role.Name = request.Name;
        var result = await roleManager.UpdateAsync(role);

        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    private static async Task<Result> DeleteRoleAsync(
        [FromServices] RoleManager<IdentityRole> roleManager,
        string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null)
        {
            return Result.Failure("Role not found.");
        }

        var result = await roleManager.DeleteAsync(role);

        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    private static async Task<Result> GetRolesAsync(
                                                    [FromServices] RoleManager<IdentityRole> roleManager,
                                                    [FromQuery] int pageNumber = 1,
                                                    [FromQuery] int pageSize = 10,
                                                    CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto(r.Id, r.Name!));

        var paginatedList = await PaginatedList<RoleDto>.CreateAsync(query, pageNumber, pageSize, cancellationToken);

        return Result.Success(paginatedList);
    }
}
