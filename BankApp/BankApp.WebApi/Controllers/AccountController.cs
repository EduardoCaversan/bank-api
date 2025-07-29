using BankApp.Application.Interfaces;
using BankApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountRepository repository) : ControllerBase
{
    private readonly IAccountRepository _repository = repository;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var account = await _repository.GetByIdAsync(id);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Account account)
    {
        try
        {
            await _repository.AddAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, Account updated)
    {
        if (id != updated.Id)
            return BadRequest("ID do corpo não corresponde ao ID da URL.");

        try
        {
            await _repository.UpdateAsync(updated);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var account = await _repository.GetByIdAsync(id);
        if (account is null) return NotFound();

        try
        {
            await _repository.DeleteAsync(account);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}