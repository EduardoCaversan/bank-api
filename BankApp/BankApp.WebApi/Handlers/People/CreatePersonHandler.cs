using BankApp.Application.Interfaces;
using BankApp.Domain.Entities;
using BankApp.WebApi.DTOs.Compliance;
using BankApp.WebApi.DTOs.People;
using BankApp.WebApi.HttpClients.Compliance;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using BankApp.Domain.DTOs;

namespace BankApp.WebApi.Handlers.People;

public class CreatePersonHandler(
    IComplianceApi complianceApi,
    IComplianceAuthApi complianceAuthApi,
    IConfiguration configuration,
    ICustomerRepository customerRepository)
{
    private readonly IComplianceApi _complianceApi = complianceApi;
    private readonly IComplianceAuthApi _complianceAuthApi = complianceAuthApi;
    private readonly IConfiguration _configuration = configuration;
    private readonly ICustomerRepository _customerRepository = customerRepository;

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
            AuthCode = authCodeResponse.Data.AuthCode
        });

        _accessToken = tokenResponse.Data.AccessToken;
        return _accessToken;
    }

    public async Task HandleAsync(CreatePersonCommand command)
    {
        var token = await GetAccessTokenAsync();

        CopmplianceValidationApiResult complianceResponse;

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

        if (complianceResponse.Data.Status != 1)
        {
            throw new InvalidOperationException(
                $"Pessoa reprovada na verificação de compliance: {complianceResponse.Data.Reason ?? "Sem motivo informado."}");
        }

        var existing = await _customerRepository.GetByDocumentAsync(command.Document);
        if (existing != null)
            throw new InvalidOperationException("Já existe um cliente com esse documento.");

        var passwordHasher = new PasswordHasher<CreateCustomerCommand>();
        var customer = new CreateCustomerCommand
        {
            Name = command.FullName,
            Document = RemoveNonDigits(command.Document),
            Email = command.Email,
        };
        customer.Password = passwordHasher.HashPassword(customer, command.Password);

        await _customerRepository.AddAsync(customer);
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