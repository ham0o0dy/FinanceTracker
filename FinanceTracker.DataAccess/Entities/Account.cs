using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.DataAccess.Entities;

public class Account
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public decimal StartAmount { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Operation> Operations { get; set; } = new List<Operation>();
    public ICollection<Transfer> TransfersFrom { get; set; } = new List<Transfer>();
    public ICollection<Transfer> TransfersTo { get; set; } = new List<Transfer>();
}