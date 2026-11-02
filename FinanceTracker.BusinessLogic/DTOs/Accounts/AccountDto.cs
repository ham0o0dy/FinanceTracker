namespace FinanceTracker.BusinessLogic.DTOs.Accounts;

public class AccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public decimal StartAmount { get; set; }
    public decimal Balance { get; set; } // R1: computed, never stored
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
}
