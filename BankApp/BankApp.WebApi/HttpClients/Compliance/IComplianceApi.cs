using Refit;
using System.Threading.Tasks;

namespace BankApp.WebApi.HttpClients.Compliance;

public interface IComplianceApi
{
    [Post("/api/compliance/check")]
    Task<ComplianceResultDto> CheckAsync([Body] ComplianceRequestDto request);
}