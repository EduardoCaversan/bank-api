using BankApp.Application.Interfaces;
using BankApp.Domain.Entities;
using BankApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Infrastructure.Repositories;

public class AccountRepository(BankAppDbContext context) : IAccountRepository
{
    private readonly BankAppDbContext _context = context;

    public async Task<IEnumerable<Account>> GetAllAsync() =>
        await _context.Accounts.Include(a => a.Transactions).ToListAsync();

    public async Task<Account?> GetByIdAsync(Guid id) =>
        await _context.Accounts.Include(a => a.Transactions)
                               .FirstOrDefaultAsync(a => a.Id == id);

    public async Task AddAsync(Account account)
    {
        var exists = await _context.Accounts
            .AnyAsync(a => a.Branch == account.Branch && a.CustomerId == account.CustomerId);
        if (exists)
            throw new InvalidOperationException("Já existe uma conta com esse número para este cliente.");

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account)
    {
        var exists = await _context.Accounts
            .AnyAsync(a => a.Branch == account.Branch && a.CustomerId == account.CustomerId);
        if (exists)
            throw new InvalidOperationException("Já existe uma conta com esse número para este cliente.");

        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Account account)
    {
        var exists = await _context.Accounts
            .AnyAsync(a => a.Branch == account.Branch && a.CustomerId == account.CustomerId);
        if (exists)
            throw new InvalidOperationException("Já existe uma conta com esse número para este cliente.");
            
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
    }
}