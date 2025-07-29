namespace BankApp.WebApi.DTOs.Compliance;

public class TokenApiResponse
{
    public bool Success { get; set; }
    public TokenResponse Data { get; set; } = default!;
}