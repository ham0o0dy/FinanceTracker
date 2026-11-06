using FinanceTracker.BusinessLogic.DTOs.Statistics;
using FinanceTracker.BusinessLogic.Exceptions;
using FinanceTracker.DataAccess;
using FinanceTracker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.BusinessLogic.Services;

public class StatisticsService(FinanceTrackerDbContext db) : IStatisticsService
{
    public async Task<StatisticsDto> GetAsync(Guid userId, StatisticsQueryParameters query)
    {
        // R16: exactly one of accountId / currency, period <= 366 days.
        var hasAccountId = query.AccountId is not null;
        var hasCurrency = !string.IsNullOrEmpty(query.Currency);

        if (hasAccountId == hasCurrency) // both or neither
            throw new BadRequestApiException("Provide exactly one of accountId or currency.");

        if (query.From > query.To)
            throw new BadRequestApiException("'from' cannot be later than 'to'.");

        if ((query.To.ToDateTime(TimeOnly.MinValue) - query.From.ToDateTime(TimeOnly.MinValue)).TotalDays > 366)
            throw new BadRequestApiException("The period cannot exceed 366 days.");

        List<Guid> accountIds;
        string currency;

        if (hasAccountId)
        {
            var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == query.AccountId)
                ?? throw new NotFoundApiException("Account not found.");

            if (account.UserId != userId)
                throw new ForbiddenApiException("This account belongs to another user.");

            accountIds = new List<Guid> { account.Id };
            currency = account.Currency;
        }
        else
        {
            currency = query.Currency!;
            accountIds = await db.Accounts
                .Where(a => a.UserId == userId && a.Currency == currency && !a.IsArchived)
                .Select(a => a.Id)
                .ToListAsync();
        }

        var operations = await db.Operations
            .Include(o => o.Category)
            .Where(o => accountIds.Contains(o.AccountId) && o.Date >= query.From && o.Date <= query.To)
            .ToListAsync();

        var totalIncome = operations.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount);
        var totalExpense = operations.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount);

        var byCategory = operations
            .GroupBy(o => new { o.CategoryId, o.Category.Name, o.Type })
            .Select(g => new
            {
                g.Key.CategoryId,
                g.Key.Name,
                g.Key.Type,
                Amount = g.Sum(o => o.Amount)
            })
            .ToList();

        var categoryStats = byCategory
            .Select(c =>
            {
                var typeTotal = c.Type == OperationType.Income ? totalIncome : totalExpense;
                var share = typeTotal == 0 ? 0 : Math.Round((double)(c.Amount / typeTotal), 4);
                return new CategoryStatistics
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.Name,
                    Type = c.Type.ToString(),
                    Amount = c.Amount,
                    Share = share
                };
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        var timeline = query.GroupBy == "month"
            ? operations
                .GroupBy(o => $"{o.Date.Year:D4}-{o.Date.Month:D2}")
                .Select(g => new TimelinePoint
                {
                    Period = g.Key,
                    Income = g.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount),
                    Expense = g.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount)
                })
                .OrderBy(p => p.Period)
                .ToList()
            : operations
                .GroupBy(o => o.Date)
                .Select(g => new TimelinePoint
                {
                    Period = g.Key.ToString("yyyy-MM-dd"),
                    Income = g.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount),
                    Expense = g.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount)
                })
                .OrderBy(p => p.Period)
                .ToList();

        return new StatisticsDto
        {
            From = query.From,
            To = query.To,
            Currency = currency,
            AccountId = hasAccountId ? query.AccountId : null,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Net = totalIncome - totalExpense,
            ByCategory = categoryStats,
            Timeline = timeline
        };
    }
}
