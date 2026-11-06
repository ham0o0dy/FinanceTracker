using FinanceTracker.BusinessLogic.DTOs.Auth;

namespace FinanceTracker.BusinessLogic.Services;

public interface IAuthService
{
    Task<TokenResponse> RegisterAsync(RegisterRequest request);
    Task<TokenResponse> LoginAsync(LoginRequest request);
}
