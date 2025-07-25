using BankApp.WebApi.DTOs.People;
using BankApp.WebApi.Handlers.People;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController(CreatePersonHandler handler) : ControllerBase
{
    private readonly CreatePersonHandler _handler = handler;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePersonCommand command)
    {
        try
        {
            await _handler.HandleAsync(command);
            return Ok(new { Message = "Pessoa criada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}