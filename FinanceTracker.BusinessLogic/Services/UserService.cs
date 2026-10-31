using FinanceTracker.BusinessLogic.DTOs.Users;
using FinanceTracker.BusinessLogic.Exceptions;
using FinanceTracker.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.BusinessLogic.Services;

public class UserService(FinanceTrackerDbContext db) : IUserService
{
    public async Task<UserDto> GetMeAsync(Guid userId)
    {
        var user = await db.Users.FindAsync(userId)
            ?? throw new NotFoundApiException("User not found.");

        return Map(user.Id, user.Email!, user.Name, user.CreatedAt);
    }

    public async Task<UserDto> UpdateMeAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await db.Users.FindAsync(userId)
            ?? throw new NotFoundApiException("User not found.");

        user.Name = request.Name;
        await db.SaveChangesAsync();

        return Map(user.Id, user.Email!, user.Name, user.CreatedAt);
    }

    private static UserDto Map(Guid id, string email, string name, DateTime createdAt) => new()
    {
        Id = id,
        Email = email,
        Name = name,
        CreatedAt = createdAt
    };
}
