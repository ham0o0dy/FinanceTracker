using FinanceTracker.BusinessLogic.DTOs.Accounts;
using FinanceTracker.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/accounts")]
[Authorize]
public class AccountsController(IAccountService accountService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAll([FromQuery] bool includeArchived = false)
    {
        return Ok(await accountService.GetAllAsync(CurrentUserId, includeArchived));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountDto>> GetById(Guid id)
    {
        return Ok(await accountService.GetByIdAsync(CurrentUserId, id));
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountRequest request)
    {
        var result = await accountService.CreateAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AccountDto>> Update(Guid id, UpdateAccountRequest request)
    {
        return Ok(await accountService.UpdateAsync(CurrentUserId, id, request));
    }

    [HttpPatch("{id:guid}/archive")]
    public async Task<ActionResult<AccountDto>> Archive(Guid id, ArchiveAccountRequest request)
    {
        return Ok(await accountService.ArchiveAsync(CurrentUserId, id, request));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await accountService.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }
}
