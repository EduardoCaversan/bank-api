namespace BankApp.WebApi.DTOs.Compliance;

public class ComplianceTransactionData
{
    public string Id { get; set; } = default!;
    public string ExternalId { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string Status { get; set; } = default!;
}