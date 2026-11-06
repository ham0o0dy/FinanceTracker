using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.BusinessLogic.DTOs.Categories;

public class UpdateCategoryRequest
{
    [Required, MinLength(1)]
    public string Name { get; set; } = null!;
}
