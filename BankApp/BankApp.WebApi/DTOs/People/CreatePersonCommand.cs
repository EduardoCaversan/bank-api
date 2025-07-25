namespace BankApp.WebApi.DTOs.People;

public class CreatePersonCommand
{
    public string FullName { get; set; } = default!;
    public string Document { get; set; } = default!;
}
