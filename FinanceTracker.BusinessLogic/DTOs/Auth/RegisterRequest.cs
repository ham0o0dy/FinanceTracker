using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.BusinessLogic.DTOs.Auth;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(1)]
    public string Name { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
