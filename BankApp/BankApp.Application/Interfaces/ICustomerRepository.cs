using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;

namespace BankApp.Application.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByDocumentAsync(string document);
    Task<Customer?> GetByEmailAsync(string document);
    Task<Customer> AddAsync(CreateCustomerCommand customer);
}