using System.Globalization;
using System.Text.Json;
using FinanceTracker.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.DataAccess.Seed;

public static class SeedData
{
    public static async Task RunAsync(FinanceTrackerDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var seed = LoadSeedFile();

        var demoUser = new ApplicationUser
        {
            Id = Guid.Parse(seed.User.Id),
            UserName = seed.User.Email,
            NormalizedUserName = seed.User.Email.ToUpperInvariant(),
            Email = seed.User.Email,
            NormalizedEmail = seed.User.Email.ToUpperInvariant(),
            EmailConfirmed = true,
            Name = seed.User.Name,
            CreatedAt = DateTime.UtcNow
        };
        demoUser.PasswordHash = new PasswordHasher<ApplicationUser>()
            .HashPassword(demoUser, seed.User.Password);
        db.Users.Add(demoUser);

        foreach (var c in seed.Categories)
        {
            db.Categories.Add(new Category
            {
                Id = Guid.Parse(c.Id),
                UserId = c.IsSystem ? null : demoUser.Id,
                Name = c.Name,
                Type = Enum.Parse<OperationType>(c.Type)
            });
        }

        foreach (var a in seed.Accounts)
        {
            db.Accounts.Add(new Account
            {
                Id = Guid.Parse(a.Id),
                UserId = demoUser.Id,
                Name = a.Name,
                Currency = a.Currency,
                StartAmount = ParseMoney(a.StartAmount),
                IsArchived = a.IsArchived,
                CreatedAt = DateTime.UtcNow
            });
        }

        foreach (var o in seed.Operations)
        {
            db.Operations.Add(new Operation
            {
                Id = Guid.Parse(o.Id),
                AccountId = Guid.Parse(o.AccountId),
                CategoryId = Guid.Parse(o.CategoryId),
                Type = Enum.Parse<OperationType>(o.Type),
                Amount = ParseMoney(o.Amount),
                Date = ParseDate(o.Date),
                Comment = o.Comment,
                CreatedAt = DateTime.UtcNow
            });
        }

        foreach (var t in seed.Transfers)
        {
            db.Transfers.Add(new Transfer
            {
                Id = Guid.Parse(t.Id),
                FromAccountId = Guid.Parse(t.FromAccountId),
                ToAccountId = Guid.Parse(t.ToAccountId),
                Amount = ParseMoney(t.Amount),
                Date = ParseDate(t.Date),
                Comment = t.Comment,
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }

    private static SeedFile LoadSeedFile()
    {
        var assembly = typeof(SeedData).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .Single(n => n.EndsWith("seed-data.json", StringComparison.OrdinalIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        return JsonSerializer.Deserialize<SeedFile>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    private static decimal ParseMoney(string value) =>
        decimal.Parse(value, CultureInfo.InvariantCulture);

    private static DateOnly ParseDate(string value) =>
        DateOnly.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    private sealed class SeedFile
    {
        public SeedUser User { get; set; } = null!;
        public List<SeedCategory> Categories { get; set; } = new();
        public List<SeedAccount> Accounts { get; set; } = new();
        public List<SeedOperation> Operations { get; set; } = new();
        public List<SeedTransfer> Transfers { get; set; } = new();
    }

    private sealed class SeedUser
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    private sealed class SeedCategory
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool IsSystem { get; set; }
    }

    private sealed class SeedAccount
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public string StartAmount { get; set; } = null!;
        public bool IsArchived { get; set; }
    }

    private sealed class SeedOperation
    {
        public string Id { get; set; } = null!;
        public string AccountId { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Amount { get; set; } = null!;
        public string Date { get; set; } = null!;
        public string? Comment { get; set; }
    }

    private sealed class SeedTransfer
    {
        public string Id { get; set; } = null!;
        public string FromAccountId { get; set; } = null!;
        public string ToAccountId { get; set; } = null!;
        public string Amount { get; set; } = null!;
        public string Date { get; set; } = null!;
        public string? Comment { get; set; }
    }
}