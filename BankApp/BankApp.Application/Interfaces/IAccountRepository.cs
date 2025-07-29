using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;

namespace BankApp.Application.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
    Task DeleteAsync(Account account);
    Task<PaginatedResponse<Account>> GetAllPaginatedAsync(int itemsPerPage, int currentPage);
}
