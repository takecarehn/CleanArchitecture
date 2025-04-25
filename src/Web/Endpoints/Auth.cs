using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Web.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", LoginAsync);
        group.MapPost("/logout", LogoutAsync);
        group.MapPost("/change-password", ChangePassAsync);
        group.MapPost("/reset-password", ResetPasssAsync);
    }

    private static async Task<Result> LoginAsync(
       [FromServices] UserManager<ApplicationUser> userManager,
       [FromServices] RoleManager<IdentityRole> roleManager,
       [FromServices] IConfiguration configuration,
       [FromBody] LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result.Failure("Invalid username or password.");
        }

        var token = GenerateJwtToken(user, configuration, userManager, roleManager);

        return Result.Success(new { User = user, Token = token });
    }

    private static string GenerateJwtToken(
                                            ApplicationUser user,
                                            IConfiguration configuration,
                                            UserManager<ApplicationUser> userManager,
                                            RoleManager<IdentityRole> roleManager)
    {
        // Các claim cơ bản định danh người dùng
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
    };

        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT key is not configured.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static async Task<Result> LogoutAsync(IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return Result.Failure("HttpContext is null.");
        }

        // Sign out the user
        await httpContext.SignOutAsync();

        // Send logout command
        return Result.Success();
    }

    private static async Task<Result> ChangePassAsync([FromServices] UserManager<ApplicationUser> userManager,
                                                      [FromBody] ChangePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null) return Result.Failure("User not found");

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(e => e.Description));
    }

    private static async Task<Result> ResetPasssAsync([FromServices] UserManager<ApplicationUser> userManager,
                                                      [FromBody] ResetPasswordRequest request)
    {
        // Tìm user theo UserId
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return Result.Failure("User not found");

        // Tạo token reset password
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        // Reset password bằng token
        var result = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

        // Trả về kết quả
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}

public record LoginRequest(string Username, string Password);
public record ChangePasswordRequest(string UserId, string CurrentPassword, string NewPassword);
public record ResetPasswordRequest(string UserId, string NewPassword);
