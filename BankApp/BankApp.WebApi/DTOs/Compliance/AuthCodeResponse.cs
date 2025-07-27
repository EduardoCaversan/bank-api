namespace BankApp.WebApi.DTOs.Compliance;

public class AuthCodeResponse
{
    public string UserId { get; set; } = default!;
    public string AuthCode { get; set; } = default!;
}