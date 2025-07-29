using BankApp.Domain.Entities;
using BankApp.Domain.DTOs;

namespace BankApp.Application.Interfaces;

public interface ITransactionRepository
{
    Task<PaginatedResponse<Transaction>> GetAllPaginatedAsync(int itemsPerPage = 10, int currentPage = 1, TransactionType? type = null);
    Task<Transaction?> GetByIdAsync(Guid id);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task DeleteAsync(Transaction transaction);
    Task RevertAsync(Guid transactionId);
}