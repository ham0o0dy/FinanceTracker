using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.DataAccess.Entities;

public class Transfer
{
    public Guid Id { get; set; }

    public Guid FromAccountId { get; set; }
    public Account FromAccount { get; set; } = null!;

    public Guid ToAccountId { get; set; }
    public Account ToAccount { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}