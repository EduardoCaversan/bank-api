using BankApp.WebApi.DTOs.Compliance;
using BankApp.WebApi.DTOs.People;
using BankApp.WebApi.HttpClients.Compliance;
using Microsoft.Extensions.Configuration;

namespace BankApp.WebApi.Handlers.People;

public class CreatePersonHandler(
    IComplianceApi complianceApi,
    IComplianceAuthApi complianceAuthApi,
    IConfiguration configuration)
{
    private readonly IComplianceApi _complianceApi = complianceApi;
    private readonly IComplianceAuthApi _complianceAuthApi = complianceAuthApi;
    private readonly IConfiguration _configuration = configuration;

    private string? _accessToken;

    private async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken))
            return _accessToken;

        var email = _configuration["ComplianceAuth:Email"];
        var password = _configuration["ComplianceAuth:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Configuração de email ou senha da Compliance API está faltando.");

        var authCodeResponse = await _complianceAuthApi.GetAuthCodeAsync(new AuthCodeRequest
        {
            Email = email,
            Password = password
        });

        var tokenResponse = await _complianceAuthApi.GetTokenAsync(new TokenRequest
        {
            AuthCode = authCodeResponse.AuthCode
        });

        _accessToken = tokenResponse.AccessToken;
        return _accessToken;
    }

    public async Task HandleAsync(CreatePersonCommand command)
    {
        var token = await GetAccessTokenAsync();

        ComplianceValidationResult complianceResponse;

        if (IsCpf(command.Document))
        {
            complianceResponse = await _complianceApi.ValidateCpfAsync(
                new ComplianceDocumentRequest { Document = command.Document },
                bearerToken: $"Bearer {token}");
        }
        else if (IsCnpj(command.Document))
        {
            complianceResponse = await _complianceApi.ValidateCnpjAsync(
                new ComplianceDocumentRequest { Document = command.Document },
                bearerToken: $"Bearer {token}");
        }
        else
        {
            throw new InvalidOperationException("Documento inválido: não é CPF nem CNPJ.");
        }

        if (complianceResponse.Status != 1)
        {
            throw new InvalidOperationException(
                $"Pessoa reprovada na verificação de compliance: {complianceResponse.Reason ?? "Sem motivo informado."}");
        }

        Console.WriteLine($"Pessoa '{command.FullName}' aprovada e registrada com documento {command.Document}.");
    }

    private static bool IsCpf(string document)
    {
        var digitsOnly = RemoveNonDigits(document);
        return digitsOnly.Length == 11;
    }

    private static bool IsCnpj(string document)
    {
        var digitsOnly = RemoveNonDigits(document);
        return digitsOnly.Length == 14;
    }

    private static string RemoveNonDigits(string input)
    {
        return new string([.. input.Where(char.IsDigit)]);
    }
}