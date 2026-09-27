using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.DataAccess.Entities;

public class Operation
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public OperationType Type { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}