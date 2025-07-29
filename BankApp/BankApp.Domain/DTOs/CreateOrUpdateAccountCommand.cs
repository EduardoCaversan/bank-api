namespace BankApp.Domain.DTOs;

public class CreateOrUpdateAccountCommand
{
    public string Branch { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public Guid CustomerId { get; set; }
}