using System;
using System.Collections.Generic;
using System.Text;
using FinanceTracker.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.DataAccess;

public class FinanceTrackerDbContext(DbContextOptions<FinanceTrackerDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<Transfer> Transfers => Set<Transfer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Account>(e =>
        {
            e.Property(a => a.StartAmount).HasColumnType("numeric(18,2)");
            e.HasIndex(a => a.UserId);
            e.HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Category>(e =>
        {
            e.HasIndex(c => new { c.UserId, c.Name, c.Type }).IsUnique();
            e.HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Operation>(e =>
        {
            e.Property(o => o.Amount).HasColumnType("numeric(18,2)");
            e.HasIndex(o => o.AccountId);
            e.HasIndex(o => o.CategoryId);
            e.HasOne(o => o.Account)
                .WithMany(a => a.Operations)
                .HasForeignKey(o => o.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.Category)
                .WithMany(c => c.Operations)
                .HasForeignKey(o => o.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Transfer>(e =>
        {
            e.Property(t => t.Amount).HasColumnType("numeric(18,2)");
            e.HasIndex(t => t.FromAccountId);
            e.HasIndex(t => t.ToAccountId);
            e.HasOne(t => t.FromAccount)
                .WithMany(a => a.TransfersFrom)
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.ToAccount)
                .WithMany(a => a.TransfersTo)
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}