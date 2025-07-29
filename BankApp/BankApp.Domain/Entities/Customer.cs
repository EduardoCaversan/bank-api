namespace BankApp.Domain.Entities;

public class Customer(string name, string email, string document, string password)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string Document { get; set; } = document;
    public string Password { get; set; } = password;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<Account> Accounts { get; set; } = [];
}
