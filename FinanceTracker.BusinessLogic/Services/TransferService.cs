using FinanceTracker.BusinessLogic.DTOs.Common;
using FinanceTracker.BusinessLogic.DTOs.Transfers;
using FinanceTracker.BusinessLogic.Exceptions;
using FinanceTracker.DataAccess;
using FinanceTracker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.BusinessLogic.Services;

public class TransferService(FinanceTrackerDbContext db) : ITransferService
{
    public async Task<PagedResult<TransferDto>> GetAllAsync(Guid userId, TransferQueryParameters query)
    {
        var baseQuery = db.Transfers
            .Include(t => t.FromAccount)
            .Include(t => t.ToAccount)
            .Where(t => t.FromAccount.UserId == userId)
            .OrderByDescending(t => t.Date).ThenByDescending(t => t.CreatedAt);

        var pageSize = Math.Clamp(query.PageSize <= 0 ? 20 : query.PageSize, 1, 100);
        var page = query.Page <= 0 ? 1 : query.Page;

        var totalCount = await baseQuery.CountAsync();
        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TransferDto>
        {
            Items = items.Select(ToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TransferDto> GetByIdAsync(Guid userId, Guid transferId)
    {
        var transfer = await FindOwnedOrThrowAsync(userId, transferId);
        return ToDto(transfer);
    }

    public async Task<TransferDto> CreateAsync(Guid userId, CreateTransferRequest request)
    {
        var fromAccount = await db.Accounts.FirstOrDefaultAsync(a => a.Id == request.FromAccountId)
            ?? throw new NotFoundApiException("Source account not found.");
        var toAccount = await db.Accounts.FirstOrDefaultAsync(a => a.Id == request.ToAccountId)
            ?? throw new NotFoundApiException("Destination account not found.");

        // R15 / R8: both accounts must belong to the requesting user.
        if (fromAccount.UserId != userId || toAccount.UserId != userId)
            throw new ForbiddenApiException("Both accounts must belong to you.");

        // R8: different accounts, same currency.
        if (fromAccount.Id == toAccount.Id)
            throw new BadRequestApiException("A transfer must be between two different accounts.");
        if (fromAccount.Currency != toAccount.Currency)
            throw new BadRequestApiException("A transfer requires both accounts to use the same currency.");

        // R2: amount > 0, at most 2 decimals.
        if (request.Amount <= 0)
            throw new BadRequestApiException("Amount must be greater than zero.");
        if (decimal.Round(request.Amount, 2) != request.Amount)
            throw new BadRequestApiException("Amount can have at most 2 decimal places.");

        // R4: date not later than today.
        if (request.Date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BadRequestApiException("Date cannot be in the future.");

        // R5: both accounts must be non-archived.
        if (fromAccount.IsArchived || toAccount.IsArchived)
            throw new ConflictApiException("Cannot create a transfer involving an archived account.");

        var transfer = new Transfer
        {
            Id = Guid.NewGuid(),
            FromAccountId = fromAccount.Id,
            ToAccountId = toAccount.Id,
            Amount = request.Amount,
            Date = request.Date,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        db.Transfers.Add(transfer);
        await db.SaveChangesAsync();

        // Reload with navigation properties for the DTO mapping.
        transfer.FromAccount = fromAccount;
        transfer.ToAccount = toAccount;

        return ToDto(transfer);
    }

    public async Task DeleteAsync(Guid userId, Guid transferId)
    {
        var transfer = await FindOwnedOrThrowAsync(userId, transferId);
        db.Transfers.Remove(transfer);
        await db.SaveChangesAsync();
    }

    private async Task<Transfer> FindOwnedOrThrowAsync(Guid userId, Guid transferId)
    {
        var transfer = await db.Transfers
            .Include(t => t.FromAccount)
            .Include(t => t.ToAccount)
            .FirstOrDefaultAsync(t => t.Id == transferId);

        if (transfer is null)
            throw new NotFoundApiException("Transfer not found.");

        if (transfer.FromAccount.UserId != userId)
            throw new ForbiddenApiException("This transfer belongs to another user.");

        return transfer;
    }

    private static TransferDto ToDto(Transfer t) => new()
    {
        Id = t.Id,
        FromAccountId = t.FromAccountId,
        FromAccountName = t.FromAccount.Name,
        ToAccountId = t.ToAccountId,
        ToAccountName = t.ToAccount.Name,
        Currency = t.FromAccount.Currency,
        Amount = t.Amount,
        Date = t.Date,
        Comment = t.Comment,
        CreatedAt = t.CreatedAt
    };
}
