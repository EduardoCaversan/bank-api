namespace BankApp.WebApi.HttpClients.Compliance;

public class ComplianceResultDto
{
    public bool IsApproved { get; set; }
    public string? Reason { get; set; }
}