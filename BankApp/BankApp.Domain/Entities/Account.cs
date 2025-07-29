namespace BankApp.Domain.Entities;

public class Account(string branch, string accountNumber, decimal balance, Guid customerId)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Branch { get; set; } = branch;
    public string AccountNumber { get; set; } = accountNumber;
    public decimal Balance { get; set; } = balance;
    public Guid CustomerId { get; set; } = customerId;
    public Customer Customer { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = [];
    public ICollection<Card> Cards { get; set; } = [];
}