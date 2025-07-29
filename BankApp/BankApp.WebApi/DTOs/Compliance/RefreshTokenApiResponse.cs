namespace BankApp.WebApi.DTOs.Compliance;

public class RefreshTokenApiResponse
{
     public bool Success { get; set; }
    public RefreshTokenResponse Data { get; set; } = default!;
}