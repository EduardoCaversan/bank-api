using BankApp.WebApi.DTOs.People;
using BankApp.WebApi.Handlers.People;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController(CreatePersonHandler createHandler, AuthenticatePersonHandler authHandler) : ControllerBase
{
    private readonly CreatePersonHandler _createHandler = createHandler;
    private readonly AuthenticatePersonHandler _authHandler = authHandler;

    [HttpPost("create")]
    public async Task<IActionResult> CreatePersonCommand([FromBody] CreatePersonCommand command)
    {
        try
        {
            await _createHandler.HandleAsync(command);
            return Ok(new { Message = "Pessoa criada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message ?? "Não foi possível criar o cliente." });
        }
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticatePersonCommand([FromBody] AuthenticatePersonCommand command)
    {
        try
        {
            var authResponse = await _authHandler.HandleAsync(command);
            return Ok(new { Message = "O login foi realizado com sucesso.", authResponse.AccessToken });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message ?? "Credenciais inválidas." });
        }
    }
}