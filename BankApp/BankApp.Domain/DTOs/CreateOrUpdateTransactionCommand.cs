using BankApp.Domain.Entities;

namespace BankApp.Domain.DTOs;

public class CreateOrUpdateTransactionCommand
{
    public decimal Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public Guid AccountId { get; set; }
    public bool IsReversal { get; set; } = false;
    public bool OriginalTransactionId { get; set; } = false;
}