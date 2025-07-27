namespace BankApp.WebApi.DTOs.Compliance;

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}