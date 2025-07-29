namespace BankApp.WebApi.DTOs.Compliance;

public class CopmplianceValidationApiResult
{
    public bool Success { get; set; }
    public ComplianceValidationResult Data { get; set; } = default!;
}