namespace BankApp.WebApi.DTOs.Compliance;

public class ComplianceValidationResult
{
    public string Document { get; set; } = default!;
    public int Status { get; set; }
    public string Reason { get; set; } = default!;
}