using BankApp.Application.Interfaces;
using BankApp.Domain.Entities;
using BankApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Infrastructure.Repositories;

public class CustomerRepository(BankAppDbContext context) : ICustomerRepository
{
    private readonly BankAppDbContext _context = context;

    public async Task<IEnumerable<Customer>> GetAllAsync() =>
        await _context.Customers.Include(c => c.Accounts).ToListAsync();

    public async Task<Customer?> GetByIdAsync(Guid id) =>
        await _context.Customers.Include(c => c.Accounts)
                                .ThenInclude(a => a.Transactions)
                                .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddAsync(Customer customer)
    {
        var exists = await _context.Customers.AnyAsync(c => c.Id == customer.Id);
        if (!exists)
            throw new InvalidOperationException("Cliente não encontrado.");

        ValidateCustomer(customer);
        
        if (await _context.Customers.AnyAsync(c => c.Document == customer.Document))
            throw new InvalidOperationException("Já existe um cliente com esse documento.");

        if (await _context.Customers.AnyAsync(c => c.Email == customer.Email))
            throw new InvalidOperationException("Já existe um cliente com esse e-mail.");

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        ValidateCustomer(customer);

        var existing = await _context.Customers.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customer.Id) ?? throw new InvalidOperationException("Cliente não encontrado.");
        if (await _context.Customers.AnyAsync(c => c.Email == customer.Email && c.Id != customer.Id))
            throw new InvalidOperationException("Já existe outro cliente com esse e-mail.");

        if (await _context.Customers.AnyAsync(c => c.Document == customer.Document && c.Id != customer.Id))
            throw new InvalidOperationException("Já existe outro cliente com esse documento.");

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Customer customer)
    {
        var exists = await _context.Customers.AnyAsync(c => c.Id == customer.Id);
        if (!exists)
            throw new InvalidOperationException("Cliente não encontrado.");
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<Customer?> GetByDocumentAsync(string document) =>
        await _context.Customers.FirstOrDefaultAsync(c => c.Document == document);

    public async Task<Customer?> GetByEmailAsync(string email) =>
        await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);

    private static void ValidateCustomer(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new InvalidOperationException("O nome do cliente é obrigatório.");

        if (string.IsNullOrWhiteSpace(customer.Email))
            throw new InvalidOperationException("O e-mail do cliente é obrigatório.");

        if (string.IsNullOrWhiteSpace(customer.Document))
            throw new InvalidOperationException("O documento do cliente é obrigatório.");
    }
}