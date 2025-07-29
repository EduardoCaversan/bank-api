namespace BankApp.Domain.DTOs;

public class CreateOrUpdateCardCommand
{
    public string Type { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
}