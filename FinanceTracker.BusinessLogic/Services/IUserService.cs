using FinanceTracker.BusinessLogic.DTOs.Users;

namespace FinanceTracker.BusinessLogic.Services;

public interface IUserService
{
    Task<UserDto> GetMeAsync(Guid userId);
    Task<UserDto> UpdateMeAsync(Guid userId, UpdateUserRequest request);
}
