using FinanceTracker.BusinessLogic.DTOs.Categories;
using FinanceTracker.BusinessLogic.Exceptions;
using FinanceTracker.DataAccess;
using FinanceTracker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.BusinessLogic.Services;

public class CategoryService(FinanceTrackerDbContext db) : ICategoryService
{
    public async Task<List<CategoryDto>> GetAllAsync(Guid userId, OperationType? type)
    {
        var query = db.Categories.Where(c => c.UserId == null || c.UserId == userId);
        if (type is not null)
            query = query.Where(c => c.Type == type);

        var categories = await query.ToListAsync();
        return categories.Select(ToDto).ToList();
    }

    public async Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryRequest request)
    {
        var type = Enum.Parse<OperationType>(request.Type);

        // R10: category names unique per user+type, case-insensitive, and can't repeat
        // a system category's name of the same type either.
        var nameLower = request.Name.Trim().ToLowerInvariant();
        var conflict = await db.Categories
            .Where(c => (c.UserId == null || c.UserId == userId) && c.Type == type)
            .AnyAsync(c => c.Name.ToLower() == nameLower);

        if (conflict)
            throw new ConflictApiException($"A category named '{request.Name}' already exists for this type.");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Type = type
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid userId, Guid categoryId, UpdateCategoryRequest request)
    {
        var category = await FindOrThrowAsync(categoryId);

        // R11: system categories cannot be renamed.
        if (category.UserId is null)
            throw new ForbiddenApiException("System categories cannot be renamed.");

        // R15: ownership.
        if (category.UserId != userId)
            throw new ForbiddenApiException("This category belongs to another user.");

        var nameLower = request.Name.Trim().ToLowerInvariant();
        var conflict = await db.Categories
            .Where(c => c.Id != categoryId && (c.UserId == null || c.UserId == userId) && c.Type == category.Type)
            .AnyAsync(c => c.Name.ToLower() == nameLower);

        if (conflict)
            throw new ConflictApiException($"A category named '{request.Name}' already exists for this type.");

        category.Name = request.Name;
        await db.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task DeleteAsync(Guid userId, Guid categoryId)
    {
        var category = await FindOrThrowAsync(categoryId);

        // R11: system categories cannot be deleted.
        if (category.UserId is null)
            throw new ForbiddenApiException("System categories cannot be deleted.");

        // R15: ownership.
        if (category.UserId != userId)
            throw new ForbiddenApiException("This category belongs to another user.");

        // R12: a category referenced by at least one operation cannot be deleted.
        var inUse = await db.Operations.AnyAsync(o => o.CategoryId == categoryId);
        if (inUse)
            throw new ConflictApiException("This category is used by an operation and cannot be deleted.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
    }

    private async Task<Category> FindOrThrowAsync(Guid categoryId)
    {
        return await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId)
            ?? throw new NotFoundApiException("Category not found.");
    }

    private static CategoryDto ToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Type = c.Type.ToString(),
        IsSystem = c.UserId == null
    };
}
