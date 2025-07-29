using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;

namespace BankApp.Application.Interfaces;

public interface ICardRepository
{
    Task<PaginatedResponse<Card>> GetAllPaginatedAsync(int itemsPerPage = 10, int currentPage = 1);
    Task<PaginatedResponse<CardMasked>> GetAllMaskedAsync(int itemsPerPage = 10, int currentPage = 1);
    Task<Card?> GetByIdAsync(Guid id);
    Task<CardMasked> AddAsync(CreateOrUpdateCardCommand card);
    Task<CardMasked> UpdateAsync(Guid id, CreateOrUpdateCardCommand card);
    Task DeleteAsync(Card card);
}