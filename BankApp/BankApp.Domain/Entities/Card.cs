namespace BankApp.Domain.Entities;

public class Card
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
}