using FinanceTracker.BusinessLogic.DTOs.Accounts;
using FinanceTracker.BusinessLogic.Exceptions;
using FinanceTracker.DataAccess;
using FinanceTracker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinanceTracker.BusinessLogic.Services;

public class AccountService(FinanceTrackerDbContext db, IConfiguration configuration) : IAccountService
{
    private string[] AllowedCurrencies =>
        configuration.GetSection("Currencies:Allowed").GetChildren().Select(c => c.Value!).ToArray();

    public async Task<List<AccountDto>> GetAllAsync(Guid userId, bool includeArchived)
    {
        var query = db.Accounts.Where(a => a.UserId == userId);
        if (!includeArchived)
            query = query.Where(a => !a.IsArchived);

        var accounts = await query.ToListAsync();
        var result = new List<AccountDto>();
        foreach (var account in accounts)
            result.Add(await ToDtoAsync(account));

        return result;
    }

    public async Task<AccountDto> GetByIdAsync(Guid userId, Guid accountId)
    {
        var account = await FindOwnedOrThrowAsync(userId, accountId);
        return await ToDtoAsync(account);
    }

    public async Task<AccountDto> CreateAsync(Guid userId, CreateAccountRequest request)
    {
        if (!AllowedCurrencies.Contains(request.Currency))
            throw new BadRequestApiException($"Currency '{request.Currency}' is not supported.");

        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Currency = request.Currency,
            StartAmount = request.StartAmount,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow
        };

        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        return await ToDtoAsync(account);
    }

    public async Task<AccountDto> UpdateAsync(Guid userId, Guid accountId, UpdateAccountRequest request)
    {
        var account = await FindOwnedOrThrowAsync(userId, accountId);

        if (!AllowedCurrencies.Contains(request.Currency))
            throw new BadRequestApiException($"Currency '{request.Currency}' is not supported.");

        // R7: currency can only change while the account has no operations and no transfers.
        if (account.Currency != request.Currency)
        {
            var hasHistory = await db.Operations.AnyAsync(o => o.AccountId == accountId)
                || await db.Transfers.AnyAsync(t => t.FromAccountId == accountId || t.ToAccountId == accountId);

            if (hasHistory)
                throw new ConflictApiException("Currency can only be changed while the account has no operations or transfers.");
        }

        account.Name = request.Name;
        account.Currency = request.Currency;
        account.StartAmount = request.StartAmount;
        await db.SaveChangesAsync();

        return await ToDtoAsync(account);
    }

    public async Task<AccountDto> ArchiveAsync(Guid userId, Guid accountId, ArchiveAccountRequest request)
    {
        var account = await FindOwnedOrThrowAsync(userId, accountId);
        account.IsArchived = request.IsArchived;
        await db.SaveChangesAsync();
        return await ToDtoAsync(account);
    }

    public async Task DeleteAsync(Guid userId, Guid accountId)
    {
        var account = await FindOwnedOrThrowAsync(userId, accountId);

        // R6: an account with at least one operation or transfer cannot be deleted.
        var hasHistory = await db.Operations.AnyAsync(o => o.AccountId == accountId)
            || await db.Transfers.AnyAsync(t => t.FromAccountId == accountId || t.ToAccountId == accountId);

        if (hasHistory)
            throw new ConflictApiException("An account with operations or transfers cannot be deleted. Archive it instead.");

        db.Accounts.Remove(account);
        await db.SaveChangesAsync();
    }

    private async Task<Account> FindOwnedOrThrowAsync(Guid userId, Guid accountId)
    {
        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account is null)
            throw new NotFoundApiException("Account not found.");

        // R15: another user's resource -> 403, not 404
        if (account.UserId != userId)
            throw new ForbiddenApiException("This account belongs to another user.");

        return account;
    }

    // R1: balance is never stored, always computed on read.
    private async Task<AccountDto> ToDtoAsync(Account account)
    {
        var incomeSum = await db.Operations
            .Where(o => o.AccountId == account.Id && o.Type == OperationType.Income)
            .SumAsync(o => (decimal?)o.Amount) ?? 0m;

        var expenseSum = await db.Operations
            .Where(o => o.AccountId == account.Id && o.Type == OperationType.Expense)
            .SumAsync(o => (decimal?)o.Amount) ?? 0m;

        var incomingTransfers = await db.Transfers
            .Where(t => t.ToAccountId == account.Id)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;

        var outgoingTransfers = await db.Transfers
            .Where(t => t.FromAccountId == account.Id)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;

        var balance = account.StartAmount + incomeSum - expenseSum + incomingTransfers - outgoingTransfers;

        return new AccountDto
        {
            Id = account.Id,
            Name = account.Name,
            Currency = account.Currency,
            StartAmount = account.StartAmount,
            Balance = balance,
            IsArchived = account.IsArchived,
            CreatedAt = account.CreatedAt
        };
    }
}
