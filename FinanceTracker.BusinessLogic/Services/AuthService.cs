using FinanceTracker.BusinessLogic.DTOs.Auth;
using FinanceTracker.BusinessLogic.Exceptions;
using FinanceTracker.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.BusinessLogic.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<TokenResponse> RegisterAsync(RegisterRequest request)
    {
        // R13: e-mail uniqueness is enforced by Identity's normalized e-mail index,
        // CreateAsync will fail with a "DuplicateEmail" error if it's taken.
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        // R14 (password policy) is enforced by Identity's configured PasswordOptions in Program.cs.
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName"))
                throw new ConflictApiException("This e-mail is already registered.");

            var message = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new BadRequestApiException(message);
        }

        var (token, expiresAt) = jwtTokenService.CreateToken(user);
        return new TokenResponse { AccessToken = token, ExpiresAt = expiresAt, RefreshToken = null };
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            throw new UnauthorizedApiException("Invalid e-mail or password.");

        var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
            throw new UnauthorizedApiException("Invalid e-mail or password.");

        var (token, expiresAt) = jwtTokenService.CreateToken(user);
        return new TokenResponse { AccessToken = token, ExpiresAt = expiresAt, RefreshToken = null };
    }
}
