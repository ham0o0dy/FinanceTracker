using FinanceTracker.BusinessLogic.DTOs.Categories;
using FinanceTracker.BusinessLogic.Services;
using FinanceTracker.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/categories")]
[Authorize]
public class CategoriesController(ICategoryService categoryService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll([FromQuery] string? type)
    {
        OperationType? parsedType = !string.IsNullOrEmpty(type) && Enum.TryParse<OperationType>(type, out var t) ? t : null;
        return Ok(await categoryService.GetAllAsync(CurrentUserId, parsedType));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request)
    {
        var result = await categoryService.CreateAsync(CurrentUserId, request);
        return StatusCode(201, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, UpdateCategoryRequest request)
    {
        return Ok(await categoryService.UpdateAsync(CurrentUserId, id, request));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await categoryService.DeleteAsync(CurrentUserId, id);
        return NoContent();
    }
}
