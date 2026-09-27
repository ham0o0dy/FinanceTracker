using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace FinanceTracker.DataAccess.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}