namespace BankApp.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;

    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
}