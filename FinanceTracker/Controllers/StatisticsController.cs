using FinanceTracker.BusinessLogic.DTOs.Statistics;
using FinanceTracker.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/statistics")]
[Authorize]
public class StatisticsController(IStatisticsService statisticsService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<StatisticsDto>> Get([FromQuery] StatisticsQueryParameters query)
    {
        return Ok(await statisticsService.GetAsync(CurrentUserId, query));
    }
}
