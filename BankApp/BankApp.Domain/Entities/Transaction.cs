namespace BankApp.Domain.Entities;

public class Transaction(decimal value, string description, TransactionType type, Guid accountId, bool isReversal = false, Guid? originalTransactionId = null)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public decimal Value { get; set; } = value;
    public string Description { get; set; } = description;
    public TransactionType Type { get; set; } = type;
    public Guid AccountId { get; set; } = accountId;
    public Account Account { get; set; } = null!;
    public bool IsReversal { get; set; } = isReversal;
    public Guid? OriginalTransactionId { get; set; } = originalTransactionId;
    public Transaction? OriginalTransaction { get; set; }
}

public enum TransactionType
{
    Credit = 0,
    Debit = 1
}