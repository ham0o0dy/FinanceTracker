using FinanceTracker.DataAccess.Entities;

namespace FinanceTracker.BusinessLogic.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(ApplicationUser user);
}
