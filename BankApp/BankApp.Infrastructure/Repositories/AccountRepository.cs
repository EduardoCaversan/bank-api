using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;
using BankApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Infrastructure.Repositories;

public class AccountRepository(BankAppDbContext context) : IAccountRepository
{
    private readonly BankAppDbContext _context = context;

    public async Task<PaginatedResponse<Account>> GetAllPaginatedAsync(int currentPage = 1, int itemsPerPage = 10)
    {
        var query = _context.Accounts
            .Include(a => a.Transactions)
            .AsQueryable();

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);

        var data = await query
            .Skip((currentPage - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToListAsync();

        return new PaginatedResponse<Account>
        {
            Data = data,
            Pagination = new PaginationMetadata
            {
                ItemsPerPage = itemsPerPage,
                CurrentPage = currentPage,
                TotalItems = totalItems,
                TotalPages = totalPages
            }
        };
    }


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