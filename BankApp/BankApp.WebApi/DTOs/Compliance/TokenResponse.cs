namespace BankApp.WebApi.DTOs.Compliance;

public class TokenResponse
{
    public string IdToken { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}