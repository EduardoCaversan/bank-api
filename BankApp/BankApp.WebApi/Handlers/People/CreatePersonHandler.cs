using BankApp.WebApi.DTOs.People;
using BankApp.WebApi.HttpClients.Compliance;

namespace BankApp.WebApi.Handlers.People;

public class CreatePersonHandler(IComplianceApi complianceApi)
{
    private readonly IComplianceApi _complianceApi = complianceApi;

    public async Task HandleAsync(CreatePersonCommand command)
    {
        var complianceResponse = await _complianceApi.CheckAsync(new ComplianceRequestDto
        {
            FullName = command.FullName,
            Document = command.Document
        });

        if (!complianceResponse.IsApproved)
        {
            throw new InvalidOperationException(
                $"Pessoa reprovada na verificação de compliance: {complianceResponse.Reason ?? "Sem motivo informado."}"
            );
        }

        // Simula persistência (você pode usar Dapper/EF aqui)
        Console.WriteLine($"Pessoa '{command.FullName}' aprovada e registrada com documento {command.Document}.");
    }
}