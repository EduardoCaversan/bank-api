namespace BankApp.WebApi.DTOs.Compliance;

public class ComplianceTransactionListResult
{
    public bool Success { get; set; }
    public List<ComplianceTransactionData> Data { get; set; } = new();
}
