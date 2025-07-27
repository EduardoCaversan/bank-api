namespace BankApp.WebApi.DTOs.Compliance;

public class ComplianceTransactionRequest
{
    public string Description { get; set; } = default!;
    public string ExternalId { get; set; } = default!;
}