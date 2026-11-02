using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.BusinessLogic.DTOs.Operations;

public class UpdateOperationRequest
{
    [Required]
    public Guid AccountId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required, RegularExpression("^(Income|Expense)$")]
    public string Type { get; set; } = null!;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}
