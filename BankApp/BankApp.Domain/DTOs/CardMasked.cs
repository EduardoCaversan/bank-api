namespace BankApp.Domain.DTOs;

public class CardMasked
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}