namespace FinanceTracker.BusinessLogic.Services;

// Holds the accepted currency codes read from configuration (Currencies:Allowed).
public class CurrencySettings(string[] allowed)
{
    public string[] Allowed { get; } = allowed;
}
