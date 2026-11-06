using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.BusinessLogic.DTOs.Accounts;

public class UpdateAccountRequest
{
    [Required, MinLength(1)]
    public string Name { get; set; } = null!;

    [Required, RegularExpression("^[A-Z]{3}$")]
    public string Currency { get; set; } = null!;

    [Required]
    public decimal StartAmount { get; set; }
}
