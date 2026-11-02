using FinanceTracker.BusinessLogic.DTOs.Categories;
using FinanceTracker.DataAccess.Entities;

namespace FinanceTracker.BusinessLogic.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(Guid userId, OperationType? type);
    Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryRequest request);
    Task<CategoryDto> UpdateAsync(Guid userId, Guid categoryId, UpdateCategoryRequest request);
    Task DeleteAsync(Guid userId, Guid categoryId);
}
