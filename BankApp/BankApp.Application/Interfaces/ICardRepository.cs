using BankApp.Domain.Entities;

namespace BankApp.Application.Interfaces;

public interface ICardRepository
{
    Task<IEnumerable<Card>> GetAllAsync();
    Task<Card?> GetByIdAsync(Guid id);
    Task AddAsync(Card card);
    Task UpdateAsync(Card card);
    Task DeleteAsync(Card card);
}