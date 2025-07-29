using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CardController(ICardRepository repository) : ControllerBase
{
    private readonly ICardRepository _repository = repository;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int itemsPerPage = 10, [FromQuery] int currentPage = 1)
    {
        var paginatedCards = await _repository.GetAllPaginatedAsync(itemsPerPage, currentPage);
        return Ok(paginatedCards);
    }

    [HttpGet("masked")]
    public async Task<IActionResult> GetMasked([FromQuery] int itemsPerPage = 10, [FromQuery] int currentPage = 1)
    {
        var paginatedCards = await _repository.GetAllMaskedAsync(itemsPerPage, currentPage);
        return Ok(paginatedCards);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var card = await _repository.GetByIdAsync(id);
        return card is null ? NotFound() : Ok(card);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateOrUpdateCardCommand card)
    {
        try
        {
            var newCard = await _repository.AddAsync(card);
            return CreatedAtAction(nameof(GetById), new { newCard.Id }, newCard);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, CreateOrUpdateCardCommand updated)
    {
        try
        {
            var updatedCard = await _repository.UpdateAsync(id, updated);
            return CreatedAtAction(nameof(GetById), new { updatedCard.Id }, updatedCard);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var card = await _repository.GetByIdAsync(id);
        if (card is null) return NotFound();

        try
        {
            await _repository.DeleteAsync(card);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}