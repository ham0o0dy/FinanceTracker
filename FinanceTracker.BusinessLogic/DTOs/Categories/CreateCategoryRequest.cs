using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.BusinessLogic.DTOs.Categories;

public class CreateCategoryRequest
{
    [Required, MinLength(1)]
    public string Name { get; set; } = null!;

    [Required, RegularExpression("^(Income|Expense)$")]
    public string Type { get; set; } = null!;
}
