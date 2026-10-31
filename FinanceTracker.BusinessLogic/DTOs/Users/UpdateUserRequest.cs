using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.BusinessLogic.DTOs.Users;

public class UpdateUserRequest
{
    [Required, MinLength(1)]
    public string Name { get; set; } = null!;
}
