namespace BankApp.WebApi.Entities;

public class BankCustomer
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
