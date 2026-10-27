namespace FinanceTracker.BusinessLogic.DTOs.Auth;

public class TokenResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string? RefreshToken { get; set; } // null unless bonus B1 is implemented
}
