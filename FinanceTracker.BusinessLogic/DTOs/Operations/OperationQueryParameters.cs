namespace FinanceTracker.BusinessLogic.DTOs.Operations;

public class OperationQueryParameters
{
    public Guid? AccountId { get; set; }
    public string? Type { get; set; } // "Income" | "Expense"
    public Guid? CategoryId { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public string SortBy { get; set; } = "dateDesc"; // dateDesc | dateAsc | amountDesc | amountAsc
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
