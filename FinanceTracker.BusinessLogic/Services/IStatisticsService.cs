using FinanceTracker.BusinessLogic.DTOs.Statistics;

namespace FinanceTracker.BusinessLogic.Services;

public interface IStatisticsService
{
    Task<StatisticsDto> GetAsync(Guid userId, StatisticsQueryParameters query);
}
