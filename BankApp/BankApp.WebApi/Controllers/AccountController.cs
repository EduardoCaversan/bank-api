using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
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
    public async Task<IActionResult> Get([FromQuery] int itemsPerPage = 10, [FromQuery] int currentPage = 1)
    {
        PaginatedResponse<Account> accounts = await _repository.GetAllPaginatedAsync(itemsPerPage, currentPage);
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var account = await _repository.GetByIdAsync(id);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateOrUpdateAccountCommand account)
    {
        try
        {
            var newAccount = await _repository.AddAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = newAccount.Id }, newAccount);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, CreateOrUpdateAccountCommand updated)
    {
        try
        {
            var updatedAccount = await _repository.UpdateAsync(id, updated);
            return CreatedAtAction(nameof(GetById), new { id = updatedAccount.Id }, updatedAccount);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}