using FinanceTracker.BusinessLogic.DTOs.Common;
using FinanceTracker.BusinessLogic.DTOs.Operations;
using FinanceTracker.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/operations")]
[Authorize]
public class OperationsController(IOperationService operationService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<OperationDto>>> GetAll([FromQuery] OperationQueryParameters query)
    {
        return Ok(await operationService.GetAllAsync(CurrentUserId, query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OperationDto>> GetById(Guid id)
    {
        return Ok(await operationService.GetByIdAsync(CurrentUserId, id));
    }

    [HttpPost]
    public async Task<ActionResult<OperationDto>> Create(CreateOperationRequest request)
    {
        var result = await operationService.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<OperationDto>> Update(Guid id, UpdateOperationRequest request)
    {
        return Ok(await operationService.UpdateAsync(CurrentUserId, id, request));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await operationService.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }
}
