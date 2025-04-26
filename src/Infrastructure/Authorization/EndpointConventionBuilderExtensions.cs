using System.Security.Claims;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Authorization;

public static class EndpointConventionBuilderExtensions
{
    public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string permission)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var httpContext = context.HttpContext;

            // 1. Kiểm tra user đã đăng nhập chưa
            if (!httpContext.User.Identity?.IsAuthenticated ?? false)
            {
                return Results.Unauthorized();
            }

            // 2. Lấy userId từ claim
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Forbid();
            }

            // 3. Lấy IIdentityService để lấy danh sách quyền
            var identityService = httpContext.RequestServices.GetRequiredService<IIdentityService>();
            var permissions = await identityService.GetUserPermissionsAsync(userId);

            // 4. Kiểm tra quyền
            if (!permissions.Contains(permission))
            {
                return Results.Forbid();
            }

            // 5. Cho qua nếu đủ quyền
            return await next(context);
        });
    }
}
