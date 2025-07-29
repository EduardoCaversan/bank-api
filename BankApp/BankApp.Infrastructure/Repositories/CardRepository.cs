using BankApp.Application.Interfaces;
using BankApp.Domain.Entities;
using BankApp.Infrastructure.Data;
using BankApp.Infrastructure.Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Infrastructure.Repositories;

public class CardRepository(BankAppDbContext context) : ICardRepository
{
    private readonly BankAppDbContext _context = context;

    public async Task<IEnumerable<Card>> GetAllAsync() =>
        await _context.Cards.Include(c => c.Account).ToListAsync();

    public async Task<Card?> GetByIdAsync(Guid id) =>
        await _context.Cards.Include(c => c.Account)
                            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<CardMaskedDto>> GetAllMaskedAsync()
    {
        var cards = await _context.Cards.ToListAsync();
        return cards.Select(c => new CardMaskedDto
        {
            Id = c.Id,
            Type = c.Type,
            Number = c.Number.Length > 4 ? c.Number[^4..] : c.Number,
            CVV = c.CVV,
            AccountId = c.AccountId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }

    public async Task<CardMaskedDto?> GetMaskedByIdAsync(Guid id)
    {
        var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
        if (card is null) return null;

        return new CardMaskedDto
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

    public async Task AddAsync(Card card)
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

        _context.Cards.Add(card);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Card card)
    {
        var existing = await _context.Cards.FindAsync(card.Id)
            ?? throw new InvalidOperationException("Cartão não encontrado.");

        if (card.Type == "physical")
        {
            var physicalExists = await _context.Cards
                .AnyAsync(c => c.Id != card.Id && c.AccountId == card.AccountId && c.Type == "physical");
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
    }

    public async Task DeleteAsync(Card card)
    {
        var existing = await _context.Cards.FindAsync(card.Id)
            ?? throw new InvalidOperationException("Cartão não encontrado.");

        _context.Cards.Remove(existing);
        await _context.SaveChangesAsync();
    }
}