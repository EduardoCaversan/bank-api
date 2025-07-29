using BankApp.Application.Interfaces;
using BankApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionController(ITransactionRepository repository) : ControllerBase
{
    private readonly ITransactionRepository _repository = repository;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int itemsPerPage = 10, [FromQuery] int currentPage = 1, [FromQuery] TransactionType? type = null)
    {
        var result = await _repository.GetAllPaginatedAsync(itemsPerPage, currentPage, type);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _repository.GetByIdAsync(id);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Transaction transaction)
    {
        try
        {
            await _repository.AddAsync(transaction);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, Transaction updated)
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
        var transaction = await _repository.GetByIdAsync(id);
        if (transaction is null) return NotFound();

        try
        {
            await _repository.DeleteAsync(transaction);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}