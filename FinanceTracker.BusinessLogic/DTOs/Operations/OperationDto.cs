namespace FinanceTracker.BusinessLogic.DTOs.Operations;

public class OperationDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid CategoryId { get; set; }
    public string Type { get; set; } = null!; // "Income" | "Expense"
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
