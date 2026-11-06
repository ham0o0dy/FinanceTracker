namespace FinanceTracker.BusinessLogic.DTOs.Transfers;

public class TransferDto
{
    public Guid Id { get; set; }
    public Guid FromAccountId { get; set; }
    public string FromAccountName { get; set; } = null!;
    public Guid ToAccountId { get; set; }
    public string ToAccountName { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
