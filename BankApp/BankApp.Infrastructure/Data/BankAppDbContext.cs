using Microsoft.EntityFrameworkCore;
using BankApp.Domain.Entities;

namespace BankApp.Infrastructure.Data;

public class BankAppDbContext : DbContext
{
    public BankAppDbContext(DbContextOptions<BankAppDbContext> options) : base(options) {}

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasMany(c => c.Accounts).WithOne(a => a.Customer);
        modelBuilder.Entity<Account>().HasMany(a => a.Transactions).WithOne(t => t.Account);
    }
}