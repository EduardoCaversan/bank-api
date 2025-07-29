using BankApp.WebApi.DTOs.Compliance;
using Refit;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BankApp.WebApi.HttpClients.Compliance;

public interface IComplianceApi
{
    [Post("/cpf/validate")]
    Task<CopmplianceValidationApiResult> ValidateCpfAsync(
        [Body] ComplianceDocumentRequest request,
        [Header("Authorization")] string bearerToken);

    [Post("/cnpj/validate")]
    Task<CopmplianceValidationApiResult> ValidateCnpjAsync(
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
    Task<AuthCodeApiResponse> GetAuthCodeAsync([Body] AuthCodeRequest request);

    [Post("/auth/token")]
    Task<TokenApiResponse> GetTokenAsync([Body] TokenRequest request);

    [Post("/auth/refresh")]
    Task<RefreshTokenApiResponse> RefreshTokenAsync([Body] RefreshTokenRequest request);
}