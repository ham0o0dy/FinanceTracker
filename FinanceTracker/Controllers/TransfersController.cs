using FinanceTracker.BusinessLogic.DTOs.Common;
using FinanceTracker.BusinessLogic.DTOs.Transfers;
using FinanceTracker.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/transfers")]
[Authorize]
public class TransfersController(ITransferService transferService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<TransferDto>>> GetAll([FromQuery] TransferQueryParameters query)
    {
        return Ok(await transferService.GetAllAsync(CurrentUserId, query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransferDto>> GetById(Guid id)
    {
        return Ok(await transferService.GetByIdAsync(CurrentUserId, id));
    }

    [HttpPost]
    public async Task<ActionResult<TransferDto>> Create(CreateTransferRequest request)
    {
        var result = await transferService.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await transferService.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }
}
