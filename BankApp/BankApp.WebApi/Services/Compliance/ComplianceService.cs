using BankApp.WebApi.DTOs.Compliance;
using BankApp.WebApi.HttpClients.Compliance;

namespace BankApp.WebApi.Services.Compliance;

public class ComplianceAuthService(IComplianceAuthApi authApi, ILogger<ComplianceAuthService> logger, IConfiguration configuration)
{
    private readonly IComplianceAuthApi _authApi = authApi;
    private readonly ILogger<ComplianceAuthService> _logger = logger;

    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _accessTokenExpiration;

    private readonly string _email = configuration["ComplianceAuth:Email"] ?? throw new ArgumentNullException("ComplianceAuth:Email");
    private readonly string _password = configuration["ComplianceAuth:Password"] ?? throw new ArgumentNullException("ComplianceAuth:Password");

    public async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _accessTokenExpiration)
            return _accessToken;

        if (_refreshToken != null)
        {
            try
            {
                var refreshResponse = await _authApi.RefreshTokenAsync(new RefreshTokenRequest
                {
                    RefreshToken = _refreshToken
                });

                _accessToken = refreshResponse.AccessToken;
                _refreshToken = refreshResponse.RefreshToken;
                _accessTokenExpiration = DateTime.UtcNow.AddMinutes(50);

                _logger.LogInformation("Token de acesso renovado via refresh token.");
                return _accessToken;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Falha ao renovar token de acesso: {Message}. Tentando obter novo token.", ex.Message);
            }
        }

        return await AuthenticateAsync();
    }

    private async Task<string> AuthenticateAsync()
    {
        var authCodeResponse = await _authApi.GetAuthCodeAsync(new AuthCodeRequest
        {
            Email = _email,
            Password = _password
        });

        var tokenResponse = await _authApi.GetTokenAsync(new TokenRequest
        {
            AuthCode = authCodeResponse.AuthCode
        });

        _accessToken = tokenResponse.AccessToken;
        _refreshToken = tokenResponse.RefreshToken;
        _accessTokenExpiration = DateTime.UtcNow.AddMinutes(50);

        _logger.LogInformation("Novo token de acesso obtido via login.");

        return _accessToken;
    }
}