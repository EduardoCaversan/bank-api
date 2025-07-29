namespace BankApp.Domain.Entities;

public class Card(string number, string cVV, string type, Guid accountId)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = type;
    public string Number { get; set; } = number;
    public string CVV { get; set; } = cVV;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid AccountId { get; set; } = accountId;
    public Account Account { get; set; } = null!;
}