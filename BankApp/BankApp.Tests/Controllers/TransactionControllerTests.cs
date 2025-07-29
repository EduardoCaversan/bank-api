using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;
using BankApp.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
    public async Task Get_ReturnsOkWithPaginatedTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new() { Id = Guid.NewGuid(), Value = 100, Type = TransactionType.Credit, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Value = 50, Type = TransactionType.Debit, CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
            new() { Id = Guid.NewGuid(), Value = 75, Type = TransactionType.Credit, CreatedAt = DateTime.UtcNow.AddMinutes(-2) }
        };

        var paginatedResponse = new PaginatedResponse<Transaction>
        {
            Data = transactions.Take(2).ToList(),
            Pagination = new PaginationMetadata
            {
                ItemsPerPage = 2,
                CurrentPage = 1,
                TotalItems = transactions.Count,
                TotalPages = 2
            }
        };

        _mockRepo.Setup(r => r.GetAllPaginatedAsync(2, 1, null)).ReturnsAsync(paginatedResponse);

        // Act
        var result = await _controller.Get(itemsPerPage: 2, currentPage: 1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedValue = Assert.IsType<PaginatedResponse<Transaction>>(okResult.Value);

        Assert.Equal(2, returnedValue.Data.Count());
        Assert.Equal(3, returnedValue.Pagination.TotalItems);
        Assert.Equal(2, returnedValue.Pagination.TotalPages);
        Assert.Equal(1, returnedValue.Pagination.CurrentPage);
    }

    [Fact]
    public async Task GetById_ReturnsOkIfFound()
    {
        var t = new Transaction { Id = Guid.NewGuid() };
        _mockRepo.Setup(r => r.GetByIdAsync(t.Id)).ReturnsAsync(t);
        var result = await _controller.GetById(t.Id);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Transaction?)null);
        var result = await _controller.GetById(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreated()
    {
        var t = new Transaction { Id = Guid.NewGuid(), AccountId = Guid.NewGuid(), Value = 100, Type = TransactionType.Credit };
        var result = await _controller.CreateAsync(t);
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBadRequest_OnException()
    {
        var t = new Transaction();
        _mockRepo.Setup(r => r.AddAsync(t)).ThrowsAsync(new InvalidOperationException("Err"));
        var result = await _controller.CreateAsync(t);
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Err", bad.Value);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsBadRequest_WhenIdMismatch()
    {
        var result = await _controller.UpdateAsync(Guid.NewGuid(), new Transaction { Id = Guid.NewGuid() });
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        var t = new Transaction { Id = id };
        var result = await _controller.UpdateAsync(id, t);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsBadRequest_OnException()
    {
        var id = Guid.NewGuid();
        var t = new Transaction { Id = id };
        _mockRepo.Setup(r => r.UpdateAsync(t)).ThrowsAsync(new InvalidOperationException("fail"));
        var result = await _controller.UpdateAsync(id, t);
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("fail", bad.Value);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Transaction?)null);
        var result = await _controller.DeleteAsync(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent()
    {
        var t = new Transaction { Id = Guid.NewGuid() };
        _mockRepo.Setup(r => r.GetByIdAsync(t.Id)).ReturnsAsync(t);
        var result = await _controller.DeleteAsync(t.Id);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsBadRequest_OnException()
    {
        var t = new Transaction { Id = Guid.NewGuid() };
        _mockRepo.Setup(r => r.GetByIdAsync(t.Id)).ReturnsAsync(t);
        _mockRepo.Setup(r => r.DeleteAsync(t)).ThrowsAsync(new InvalidOperationException("delete fail"));
        var result = await _controller.DeleteAsync(t.Id);
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("delete fail", bad.Value);
    }
}