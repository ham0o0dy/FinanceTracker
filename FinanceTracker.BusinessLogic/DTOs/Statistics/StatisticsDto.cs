namespace FinanceTracker.BusinessLogic.DTOs.Statistics;

public class CategoryStatistics
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string Type { get; set; } = null!;
    public decimal Amount { get; set; }
    public double Share { get; set; }
}

public class TimelinePoint
{
    public string Period { get; set; } = null!;
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}

public class StatisticsDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public string Currency { get; set; } = null!;
    public Guid? AccountId { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Net { get; set; }
    public List<CategoryStatistics> ByCategory { get; set; } = new();
    public List<TimelinePoint> Timeline { get; set; } = new();
}

public class StatisticsQueryParameters
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public Guid? AccountId { get; set; }
    public string? Currency { get; set; }
    public string GroupBy { get; set; } = "day"; // day | month
}
