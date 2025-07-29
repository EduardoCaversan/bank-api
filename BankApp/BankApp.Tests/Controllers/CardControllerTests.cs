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
        var card = new Card("1234567812345678", "123", "virtual", Guid.NewGuid());
        _mockRepo.Setup(r => r.GetByIdAsync(card.Id)).ReturnsAsync(card);

        var result = await _controller.GetById(card.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(card, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Card?)null);

        var result = await _controller.GetById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_OnException()
    {
        var dto = new CreateOrUpdateCardCommand
        {
            Number = "1234",
            CVV = "12",
            Type = "virtual",
            AccountId = Guid.NewGuid()
        };

        _mockRepo.Setup(r => r.AddAsync(dto)).ThrowsAsync(new InvalidOperationException("Invalid"));

        var result = await _controller.CreateAsync(dto);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid", badResult.Value);
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
        var card = new Card("1234567890123456", "123", "virtual", Guid.NewGuid());
        _mockRepo.Setup(r => r.GetByIdAsync(card.Id)).ReturnsAsync(card);

        var result = await _controller.DeleteAsync(card.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsBadRequest_OnException()
    {
        var card = new Card("1234567890123456", "123", "virtual", Guid.NewGuid());
        _mockRepo.Setup(r => r.GetByIdAsync(card.Id)).ReturnsAsync(card);
        _mockRepo.Setup(r => r.DeleteAsync(card)).ThrowsAsync(new InvalidOperationException("Del fail"));

        var result = await _controller.DeleteAsync(card.Id);

        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Del fail", badResult.Value);
    }
}