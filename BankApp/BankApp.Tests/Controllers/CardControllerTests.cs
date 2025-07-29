using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;
using BankApp.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankApp.Tests.Controllers;

public class CardControllerTests
{
    private readonly Mock<ICardRepository> _mockRepo = new();
    private readonly CardController _controller;

    public CardControllerTests()
    {
        _controller = new CardController(_mockRepo.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkWithCards()
    {
        _mockRepo.Setup(r => r.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>()))
                 .ReturnsAsync(new PaginatedResponse<Card> { Data = [], Pagination = new() });

        var result = await _controller.Get();

        Assert.IsType<OkObjectResult>(result);
    }


    [Fact]
    public async Task GetById_ReturnsOkIfFound()
    {
        var card = new Card { Id = Guid.NewGuid() };
        _mockRepo.Setup(r => r.GetByIdAsync(card.Id)).ReturnsAsync(card);
        var result = await _controller.GetById(card.Id);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Card?)null);
        var result = await _controller.GetById(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreated()
    {
        var card = new Card { Id = Guid.NewGuid(), Number = "1234567812345678", CVV = "123", AccountId = Guid.NewGuid(), Type = "virtual" };
        var result = await _controller.CreateAsync(card);
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_OnException()
    {
        var card = new Card();
        _mockRepo.Setup(r => r.AddAsync(card)).ThrowsAsync(new InvalidOperationException("Invalid"));
        var result = await _controller.CreateAsync(card);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid", badResult.Value);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsBadRequest_WhenIdMismatch()
    {
        var result = await _controller.UpdateAsync(Guid.NewGuid(), new Card { Id = Guid.NewGuid() });
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        var card = new Card { Id = id };
        var result = await _controller.UpdateAsync(id, card);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsBadRequest_OnException()
    {
        var id = Guid.NewGuid();
        var card = new Card { Id = id };
        _mockRepo.Setup(r => r.UpdateAsync(card)).ThrowsAsync(new InvalidOperationException("Update fail"));
        var result = await _controller.UpdateAsync(id, card);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Update fail", badResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Card?)null);
        var result = await _controller.DeleteAsync(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent()
    {
        var card = new Card { Id = Guid.NewGuid() };
        _mockRepo.Setup(r => r.GetByIdAsync(card.Id)).ReturnsAsync(card);
        var result = await _controller.DeleteAsync(card.Id);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsBadRequest_OnException()
    {
        var card = new Card { Id = Guid.NewGuid() };
        _mockRepo.Setup(r => r.GetByIdAsync(card.Id)).ReturnsAsync(card);
        _mockRepo.Setup(r => r.DeleteAsync(card)).ThrowsAsync(new InvalidOperationException("Del fail"));
        var result = await _controller.DeleteAsync(card.Id);
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Del fail", badResult.Value);
    }
}