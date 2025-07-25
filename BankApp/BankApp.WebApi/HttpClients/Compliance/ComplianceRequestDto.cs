namespace BankApp.WebApi.HttpClients.Compliance;

public class ComplianceRequestDto
{
    public string FullName { get; set; } = default!;
    public string Document { get; set; } = default!;
}