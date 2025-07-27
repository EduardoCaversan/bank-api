namespace BankApp.WebApi.DTOs.Compliance;

public class ComplianceTransactionResult
{
    public bool Success { get; set; }
    public ComplianceTransactionData Data { get; set; } = default!;
}