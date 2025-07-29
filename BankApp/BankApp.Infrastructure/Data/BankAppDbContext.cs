using Microsoft.EntityFrameworkCore;
using BankApp.Domain.Entities;

namespace BankApp.Infrastructure.Data;

public class BankAppDbContext(DbContextOptions<BankAppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Card> Cards => Set<Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasMany(c => c.Accounts).WithOne(a => a.Customer);
        modelBuilder.Entity<Account>().HasMany(a => a.Transactions).WithOne(t => t.Account);
        modelBuilder.Entity<Account>().HasMany(a => a.Cards).WithOne(c => c.Account);
    }
}