namespace BankApp.WebApi.DTOs.Compliance;

public class AuthCodeRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}