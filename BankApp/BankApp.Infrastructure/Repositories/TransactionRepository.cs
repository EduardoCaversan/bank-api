using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;
using BankApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Infrastructure.Repositories;

public class TransactionRepository(BankAppDbContext context) : ITransactionRepository
{
    private readonly BankAppDbContext _context = context;

    public async Task<PaginatedResponse<Transaction>> GetAllPaginatedAsync(int itemsPerPage = 10, int currentPage = 1, TransactionType? type = null)
    {
        var query = _context.Transactions.Include(t => t.Account).AsQueryable();

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        var totalItems = await query.CountAsync();
        var data = await query.OrderByDescending(t => t.CreatedAt)
                              .Skip((currentPage - 1) * itemsPerPage)
                              .Take(itemsPerPage)
                              .ToListAsync();

        return new PaginatedResponse<Transaction>
        {
            Data = data,
            Pagination = new PaginationMetadata
            {
                ItemsPerPage = itemsPerPage,
                CurrentPage = currentPage,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage)
            }
        };
    }

    public async Task<Transaction?> GetByIdAsync(Guid id) =>
        await _context.Transactions.Include(t => t.Account)
                                   .FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddAsync(Transaction transaction)
    {
        var account = await _context.Accounts.FindAsync(transaction.AccountId)
            ?? throw new InvalidOperationException("Conta não encontrada.");

        if (transaction.Value <= 0)
            throw new InvalidOperationException("Valor da transação deve ser maior que zero.");

        if (transaction.Type == TransactionType.Debit && account.Balance < transaction.Value)
            throw new InvalidOperationException("Saldo insuficiente para débito.");

        if (transaction.Type == TransactionType.Debit)
            account.Balance -= transaction.Value;
        else
            account.Balance += transaction.Value;

        transaction.CreatedAt = DateTime.UtcNow;
        transaction.IsReversal = false;

        _context.Transactions.Add(transaction);
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        var existing = await _context.Transactions.FindAsync(transaction.Id)
            ?? throw new InvalidOperationException("Transação não encontrada.");

        if (existing.IsReversal)
            throw new InvalidOperationException("Transações revertidas não podem ser atualizadas.");
        throw new NotSupportedException("Atualização de transações não é permitida.");
    }

    public async Task DeleteAsync(Transaction transaction)
    {
        var existing = await _context.Transactions.FindAsync(transaction.Id)
            ?? throw new InvalidOperationException("Transação não encontrada.");

        if (existing.IsReversal)
            throw new InvalidOperationException("Transações revertidas não podem ser removidas.");

        _context.Transactions.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task RevertAsync(Guid transactionId)
    {
        var original = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == transactionId)
            ?? throw new InvalidOperationException("Transação original não encontrada.");

        if (original.IsReversal)
            throw new InvalidOperationException("Transação já foi revertida.");

        var account = original.Account!;

        var reverseType = original.Type == TransactionType.Credit ? TransactionType.Debit : TransactionType.Credit;

        if (reverseType == TransactionType.Debit && account.Balance < original.Value)
            throw new InvalidOperationException("Saldo insuficiente para reverter.");

        var reverseTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            CreatedAt = DateTime.UtcNow,
            Description = $"Reversão: {original.Description}",
            Type = reverseType,
            Value = original.Value,
            IsReversal = false
        };

        if (reverseType == TransactionType.Debit)
            account.Balance -= original.Value;
        else
            account.Balance += original.Value;

        original.IsReversal = true;

        _context.Transactions.Add(reverseTransaction);
        _context.Transactions.Update(original);
        _context.Accounts.Update(account);

        await _context.SaveChangesAsync();
    }
}
