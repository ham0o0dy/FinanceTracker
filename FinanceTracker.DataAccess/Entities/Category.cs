using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.DataAccess.Entities;

public class Category
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public string Name { get; set; } = null!;
    public OperationType Type { get; set; }

    public ICollection<Operation> Operations { get; set; } = new List<Operation>();
}