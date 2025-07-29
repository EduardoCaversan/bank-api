using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;

namespace BankApp.Application.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account> AddAsync(CreateOrUpdateAccountCommand account);
    Task<Account> UpdateAsync(Guid id, CreateOrUpdateAccountCommand account);
    Task DeleteAsync(Guid id);
    Task<PaginatedResponse<Account>> GetAllPaginatedAsync(int itemsPerPage, int currentPage);
}
