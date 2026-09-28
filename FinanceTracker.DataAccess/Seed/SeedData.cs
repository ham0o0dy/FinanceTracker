using FinanceTracker.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.DataAccess.Seed;

public static class SeedData
{
    public static async Task RunAsync(FinanceTrackerDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var demoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var demoUser = new ApplicationUser
        {
            Id = demoUserId,
            UserName = "demo@fintracker.local",
            NormalizedUserName = "DEMO@FINTRACKER.LOCAL",
            Email = "demo@fintracker.local",
            NormalizedEmail = "DEMO@FINTRACKER.LOCAL",
            EmailConfirmed = true,
            Name = "Demo User",
            CreatedAt = DateTime.UtcNow
        };
        demoUser.PasswordHash = new PasswordHasher<ApplicationUser>()
            .HashPassword(demoUser, "Demo12345");

        db.Users.Add(demoUser);

        db.Categories.AddRange(
            new Category { Id = Guid.Parse("a0000000-0000-0000-0000-000000000001"), Name = "Salary", Type = OperationType.Income },
            new Category { Id = Guid.Parse("a0000000-0000-0000-0000-000000000002"), Name = "Scholarship", Type = OperationType.Income },
            new Category { Id = Guid.Parse("a0000000-0000-0000-0000-000000000003"), Name = "Other", Type = OperationType.Income },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000001"), Name = "Food", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000002"), Name = "Restaurants", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000003"), Name = "Medicine", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000004"), Name = "Sport", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000005"), Name = "Taxi", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000006"), Name = "Rent", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000007"), Name = "Investments", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000008"), Name = "Clothes", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000009"), Name = "Fun", Type = OperationType.Expense },
            new Category { Id = Guid.Parse("b0000000-0000-0000-0000-000000000010"), Name = "Other", Type = OperationType.Expense }
        );

        db.Accounts.AddRange(
            new Account { Id = Guid.Parse("c0000000-0000-0000-0000-000000000001"), UserId = demoUserId, Name = "Cash", Currency = "RUB", StartAmount = 5000.00m, CreatedAt = DateTime.UtcNow },
            new Account { Id = Guid.Parse("c0000000-0000-0000-0000-000000000002"), UserId = demoUserId, Name = "Debit card", Currency = "RUB", StartAmount = 42000.00m, CreatedAt = DateTime.UtcNow },
            new Account { Id = Guid.Parse("c0000000-0000-0000-0000-000000000003"), UserId = demoUserId, Name = "Savings", Currency = "USD", StartAmount = 1500.00m, CreatedAt = DateTime.UtcNow }
        );

        await db.SaveChangesAsync();

    }
}