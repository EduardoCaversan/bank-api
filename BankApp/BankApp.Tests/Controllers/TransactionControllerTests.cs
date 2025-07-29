using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;
using BankApp.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BankApp.Tests.Controllers;

public class TransactionControllerTests
{
    private readonly Mock<ITransactionRepository> _mockRepo = new();
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
        _controller = new TransactionController(_mockRepo.Object);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Transaction?)null);

        var result = await _controller.GetById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_OnException()
    {
        var command = new CreateOrUpdateTransactionCommand();

        _mockRepo.Setup(r => r.AddAsync(command))
                 .ThrowsAsync(new InvalidOperationException("Err"));

        var result = await _controller.CreateAsync(command);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Err", badRequest.Value);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenSuccessful()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteAsync(id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsBadRequest_OnException()
    {
        var id = Guid.NewGuid();

        _mockRepo.Setup(r => r.DeleteAsync(id))
                 .ThrowsAsync(new InvalidOperationException("delete fail"));

        var result = await _controller.DeleteAsync(id);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("delete fail", bad.Value);
    }
}