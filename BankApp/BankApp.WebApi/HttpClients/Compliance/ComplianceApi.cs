using BankApp.WebApi.DTOs.Compliance;
using Refit;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BankApp.WebApi.HttpClients.Compliance;

public interface IComplianceApi
{
    [Post("/cpf/validate")]
    Task<ComplianceValidationResult> ValidateCpfAsync(
        [Body] ComplianceDocumentRequest request,
        [Header("Authorization")] string bearerToken);

    [Post("/cnpj/validate")]
    Task<ComplianceValidationResult> ValidateCnpjAsync(
        [Body] ComplianceDocumentRequest request,
        [Header("Authorization")] string bearerToken);

    [Put("/transaction/{id}")]
    Task<ComplianceTransactionResult> CreateTransactionAsync(
        string id,
        [Body] ComplianceTransactionRequest request,
        [Header("Authorization")] string bearerToken);

    [Get("/transaction/{id}")]
    Task<ComplianceTransactionResult> GetTransactionByIdAsync(
        string id,
        [Header("Authorization")] string bearerToken);

    [Get("/transaction")]
    Task<ComplianceTransactionListResult> GetTransactionsAsync(
        [Header("Authorization")] string bearerToken);
}

public interface IComplianceAuthApi
{
    [Post("/auth/code")]
    Task<AuthCodeResponse> GetAuthCodeAsync([Body] AuthCodeRequest request);

    [Post("/auth/token")]
    Task<TokenResponse> GetTokenAsync([Body] TokenRequest request);

    [Post("/auth/refresh")]
    Task<RefreshTokenResponse> RefreshTokenAsync([Body] RefreshTokenRequest request);
}