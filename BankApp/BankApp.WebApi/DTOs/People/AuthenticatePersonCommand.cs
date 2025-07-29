namespace BankApp.WebApi.DTOs.People;

public class AuthenticatePersonCommand
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}