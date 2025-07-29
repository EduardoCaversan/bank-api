using BankApp.Domain.Entities;
using BankApp.Domain.DTOs;

namespace BankApp.Application.Interfaces;

public interface ITransactionRepository
{
    Task<PaginatedResponse<Transaction>> GetAllPaginatedAsync(int itemsPerPage = 10, int currentPage = 1, TransactionType? type = null);
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<Transaction> AddAsync(CreateOrUpdateTransactionCommand transaction);
    Task UpdateAsync(Guid id, CreateOrUpdateTransactionCommand transaction);
    Task DeleteAsync(Guid id);
    Task RevertAsync(Guid transactionId);
}