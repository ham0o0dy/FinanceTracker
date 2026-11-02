using FinanceTracker.BusinessLogic.DTOs.Common;
using FinanceTracker.BusinessLogic.DTOs.Operations;
using FinanceTracker.BusinessLogic.Exceptions;
using FinanceTracker.DataAccess;
using FinanceTracker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.BusinessLogic.Services;

public class OperationService(FinanceTrackerDbContext db) : IOperationService
{
    public async Task<PagedResult<OperationDto>> GetAllAsync(Guid userId, OperationQueryParameters query)
    {
        var baseQuery = db.Operations
            .Include(o => o.Account)
            .Where(o => o.Account.UserId == userId);

        if (query.AccountId is not null)
            baseQuery = baseQuery.Where(o => o.AccountId == query.AccountId);

        if (!string.IsNullOrEmpty(query.Type) && Enum.TryParse<OperationType>(query.Type, out var type))
            baseQuery = baseQuery.Where(o => o.Type == type);

        if (query.CategoryId is not null)
            baseQuery = baseQuery.Where(o => o.CategoryId == query.CategoryId);

        if (query.From is not null)
            baseQuery = baseQuery.Where(o => o.Date >= query.From);

        if (query.To is not null)
            baseQuery = baseQuery.Where(o => o.Date <= query.To);

        if (query.From is not null && query.To is not null && query.From > query.To)
            throw new BadRequestApiException("'from' cannot be later than 'to'.");

        baseQuery = query.SortBy switch
        {
            "dateAsc" => baseQuery.OrderBy(o => o.Date).ThenByDescending(o => o.CreatedAt),
            "amountDesc" => baseQuery.OrderByDescending(o => o.Amount).ThenByDescending(o => o.CreatedAt),
            "amountAsc" => baseQuery.OrderBy(o => o.Amount).ThenByDescending(o => o.CreatedAt),
            _ => baseQuery.OrderByDescending(o => o.Date).ThenByDescending(o => o.CreatedAt), // dateDesc default
        };

        var pageSize = Math.Clamp(query.PageSize <= 0 ? 20 : query.PageSize, 1, 100);
        var page = query.Page <= 0 ? 1 : query.Page;

        var totalCount = await baseQuery.CountAsync();
        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OperationDto>
        {
            Items = items.Select(ToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<OperationDto> GetByIdAsync(Guid userId, Guid operationId)
    {
        var operation = await FindOwnedOrThrowAsync(userId, operationId);
        return ToDto(operation);
    }

    public async Task<OperationDto> CreateAsync(Guid userId, CreateOperationRequest request)
    {
        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId)
            ?? throw new NotFoundApiException("Account not found.");

        if (account.UserId != userId)
            throw new ForbiddenApiException("This account belongs to another user.");

        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId)
            ?? throw new NotFoundApiException("Category not found.");

        if (category.UserId is not null && category.UserId != userId)
            throw new ForbiddenApiException("This category belongs to another user.");

        var type = Enum.Parse<OperationType>(request.Type);

        ValidateOperationRules(request.Amount, request.Date, type, category.Type, account.IsArchived);

        var operation = new Operation
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            CategoryId = category.Id,
            Type = type,
            Amount = request.Amount,
            Date = request.Date,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        db.Operations.Add(operation);
        await db.SaveChangesAsync();

        return ToDto(operation);
    }

    public async Task<OperationDto> UpdateAsync(Guid userId, Guid operationId, UpdateOperationRequest request)
    {
        var operation = await FindOwnedOrThrowAsync(userId, operationId);

        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId)
            ?? throw new NotFoundApiException("Account not found.");

        if (account.UserId != userId)
            throw new ForbiddenApiException("This account belongs to another user.");

        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId)
            ?? throw new NotFoundApiException("Category not found.");

        if (category.UserId is not null && category.UserId != userId)
            throw new ForbiddenApiException("This category belongs to another user.");

        var type = Enum.Parse<OperationType>(request.Type);

        ValidateOperationRules(request.Amount, request.Date, type, category.Type, account.IsArchived);

        operation.AccountId = account.Id;
        operation.CategoryId = category.Id;
        operation.Type = type;
        operation.Amount = request.Amount;
        operation.Date = request.Date;
        operation.Comment = request.Comment;

        await db.SaveChangesAsync();

        return ToDto(operation);
    }

    public async Task DeleteAsync(Guid userId, Guid operationId)
    {
        // Per the assignment's clarifications: existing operations of an archived account
        // remain readable and CAN be deleted, so no archived-account check here.
        var operation = await FindOwnedOrThrowAsync(userId, operationId);
        db.Operations.Remove(operation);
        await db.SaveChangesAsync();
    }

    private static void ValidateOperationRules(decimal amount, DateOnly date, OperationType requestedType, OperationType categoryType, bool accountIsArchived)
    {
        // R2: amount > 0, at most 2 decimal places.
        if (amount <= 0)
            throw new BadRequestApiException("Amount must be greater than zero.");
        if (decimal.Round(amount, 2) != amount)
            throw new BadRequestApiException("Amount can have at most 2 decimal places.");

        // R3: operation type must match category type.
        if (requestedType != categoryType)
            throw new BadRequestApiException("The operation type must match the category's type.");

        // R4: date not later than today (UTC).
        if (date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new BadRequestApiException("Date cannot be in the future.");

        // R5: operations are created/moved only on non-archived accounts.
        if (accountIsArchived)
            throw new ConflictApiException("Cannot add or move an operation to an archived account.");
    }

    private async Task<Operation> FindOwnedOrThrowAsync(Guid userId, Guid operationId)
    {
        var operation = await db.Operations
            .Include(o => o.Account)
            .FirstOrDefaultAsync(o => o.Id == operationId);

        if (operation is null)
            throw new NotFoundApiException("Operation not found.");

        if (operation.Account.UserId != userId)
            throw new ForbiddenApiException("This operation belongs to another user.");

        return operation;
    }

    private static OperationDto ToDto(Operation o) => new()
    {
        Id = o.Id,
        AccountId = o.AccountId,
        CategoryId = o.CategoryId,
        Type = o.Type.ToString(),
        Amount = o.Amount,
        Date = o.Date,
        Comment = o.Comment,
        CreatedAt = o.CreatedAt
    };
}
