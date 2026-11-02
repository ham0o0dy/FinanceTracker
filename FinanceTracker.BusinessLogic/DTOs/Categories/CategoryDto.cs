namespace FinanceTracker.BusinessLogic.DTOs.Categories;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!; // "Income" | "Expense"
    public bool IsSystem { get; set; }
}
