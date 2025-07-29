namespace BankApp.Domain.DTOs;

public class CreateCustomerCommand
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}