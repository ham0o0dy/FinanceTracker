using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.BusinessLogic.DTOs.Operations;

public class CreateOperationRequest
{
    [Required]
    public Guid AccountId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required, RegularExpression("^(Income|Expense)$")]
    public string Type { get; set; } = null!;

    // R2: amount > 0, at most 2 decimals. The >0 part is checked here;
    // the "at most 2 decimals" part is checked in the service (decimal.Round comparison),
    // since DataAnnotations doesn't have a clean built-in attribute for that.
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}
