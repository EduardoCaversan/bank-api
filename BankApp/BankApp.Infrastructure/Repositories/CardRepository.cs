using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;
using BankApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Infrastructure.Repositories;

public class CardRepository(BankAppDbContext context) : ICardRepository
{
    private readonly BankAppDbContext _context = context;

    public async Task<PaginatedResponse<Card>> GetAllPaginatedAsync(int itemsPerPage = 10, int currentPage = 1)
    {
        var query = _context.Cards.Include(c => c.Account).AsQueryable();
        var totalItems = await query.CountAsync();
        var data = await query.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

        return new PaginatedResponse<Card>
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

    public async Task<Card?> GetByIdAsync(Guid id) =>
        await _context.Cards.Include(c => c.Account)
                            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<PaginatedResponse<CardMasked>> GetAllMaskedAsync(int itemsPerPage = 10, int currentPage = 1)
    {
        var query = _context.Cards.Include(c => c.Account).AsQueryable();
        var totalItems = await query.CountAsync();
        var data = await query.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();

        var maskedCards = data.Select(c => new CardMasked
        {
            Id = c.Id,
            Type = c.Type,
            Number = c.Number.Length > 4 ? c.Number[^4..] : c.Number,
            CVV = c.CVV,
            AccountId = c.AccountId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        return new PaginatedResponse<CardMasked>
        {
            Data = maskedCards,
            Pagination = new PaginationMetadata
            {
                ItemsPerPage = itemsPerPage,
                CurrentPage = currentPage,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage)
            }
        };
    }

    public async Task<CardMasked?> GetMaskedByIdAsync(Guid id)
    {
        var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
        if (card is null) return null;

        return new CardMasked
        {
            Id = card.Id,
            Type = card.Type,
            Number = card.Number.Length > 4 ? card.Number[^4..] : card.Number,
            CVV = card.CVV,
            AccountId = card.AccountId,
            CreatedAt = card.CreatedAt,
            UpdatedAt = card.UpdatedAt
        };
    }

    public async Task<CardMasked> AddAsync(CreateOrUpdateCardCommand card)
    {
        var account = await _context.Accounts.FindAsync(card.AccountId)
            ?? throw new InvalidOperationException("Conta associada ao cartão não encontrada.");

        if (card.Type == "physical")
        {
            var physicalExists = await _context.Cards
                .AnyAsync(c => c.AccountId == card.AccountId && c.Type == "physical");
            if (physicalExists)
                throw new InvalidOperationException("Já existe um cartão físico para esta conta.");
        }

        if (card.CVV.Length != 3 || !card.CVV.All(char.IsDigit))
            throw new InvalidOperationException("CVV deve conter exatamente 3 dígitos numéricos.");

        if (string.IsNullOrWhiteSpace(card.Number) || card.Number.Length < 4)
            throw new InvalidOperationException("Número do cartão inválido.");
        var newCard = new Card(card.Number, card.CVV, card.Type, card.AccountId);
        _context.Cards.Add(newCard);
        await _context.SaveChangesAsync();
        return new CardMasked
        {
            Id = newCard.Id,
            Type = newCard.Type,
            Number = newCard.Number.Length > 4 ? newCard.Number[^4..] : newCard.Number,
            CVV = newCard.CVV,
            AccountId = newCard.AccountId,
            CreatedAt = newCard.CreatedAt,
            UpdatedAt = newCard.UpdatedAt
        };
    }

    public async Task<CardMasked> UpdateAsync(Guid id, CreateOrUpdateCardCommand card)
    {
        var existing = await _context.Cards.FindAsync(id)
            ?? throw new InvalidOperationException("Cartão não encontrado.");

        if (card.Type == "physical")
        {
            var physicalExists = await _context.Cards
                .AnyAsync(c => c.Id != id && c.AccountId == card.AccountId && c.Type == "physical");
            if (physicalExists)
                throw new InvalidOperationException("Já existe outro cartão físico para esta conta.");
        }

        if (card.CVV.Length != 3 || !card.CVV.All(char.IsDigit))
            throw new InvalidOperationException("CVV deve conter exatamente 3 dígitos numéricos.");

        existing.Type = card.Type;
        existing.CVV = card.CVV;
        existing.Number = card.Number;

        _context.Cards.Update(existing);
        await _context.SaveChangesAsync();
        return new CardMasked
        {
            Id = existing.Id,
            Type = existing.Type,
            Number = existing.Number.Length > 4 ? existing.Number[^4..] : existing.Number,
            CVV = existing.CVV,
            AccountId = existing.AccountId,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = existing.UpdatedAt
        };
    }

    public async Task DeleteAsync(Card card)
    {
        var existing = await _context.Cards.FindAsync(card.Id)
            ?? throw new InvalidOperationException("Cartão não encontrado.");

        _context.Cards.Remove(existing);
        await _context.SaveChangesAsync();
    }
}