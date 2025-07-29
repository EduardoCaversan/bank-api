namespace BankApp.WebApi.DTOs.Compliance;

public class AuthCodeApiResponse
{
    public bool Success { get; set; }
    public AuthCodeResponse Data { get; set; } = default!;
}