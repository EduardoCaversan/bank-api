namespace BankApp.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public decimal Value { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public bool IsReversal { get; set; } = false;
    public Guid? OriginalTransactionId { get; set; }
    public Transaction? OriginalTransaction { get; set; }
}

public enum TransactionType
{
    Credit,
    Debit
}