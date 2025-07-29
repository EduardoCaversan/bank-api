using BankApp.Application.Interfaces;
using BankApp.Domain.DTOs;
using BankApp.Domain.Entities;
using BankApp.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankApp.Tests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IAccountRepository> _mockRepo;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _mockRepo = new Mock<IAccountRepository>();
        _controller = new AccountController(_mockRepo.Object);
    }

    [Fact]
    public async Task Get_ReturnsPaginatedAccounts()
    {
        // Arrange
        var paginatedResult = new PaginatedResponse<Account>
        {
            Data = [new Account("0001", "12345-6", 500.00m, Guid.NewGuid())],
            Pagination = new PaginationMetadata{ ItemsPerPage = 10, CurrentPage = 1, TotalItems = 1, TotalPages = 1 }
        };

        _mockRepo.Setup(r => r.GetAllPaginatedAsync(10, 1))
                 .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.Get(10, 1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(paginatedResult, okResult.Value);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsAccount()
    {
        // Arrange
        var id = Guid.NewGuid();
        var account = new Account("0001", "12345-6", 1000.00m, Guid.NewGuid()) { Id = id };

        _mockRepo.Setup(r => r.GetByIdAsync(id))
                 .ReturnsAsync(account);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(account, okResult.Value);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Account?)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsCreatedAccount()
    {
        // Arrange
        var command = new CreateOrUpdateAccountCommand
        {
            Branch = "0001",
            AccountNumber = "12345-6",
            Balance = 2000.00m,
            CustomerId = Guid.NewGuid()
        };

        var createdAccount = new Account(command.Branch, command.AccountNumber, command.Balance, command.CustomerId);

        _mockRepo.Setup(r => r.AddAsync(command)).ReturnsAsync(createdAccount);

        // Act
        var result = await _controller.CreateAsync(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(createdAccount, createdResult.Value);
    }

    [Fact]
    public async Task CreateAsync_InvalidOperation_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateOrUpdateAccountCommand();
        _mockRepo.Setup(r => r.AddAsync(command)).ThrowsAsync(new InvalidOperationException("Erro"));

        // Act
        var result = await _controller.CreateAsync(command);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erro", badRequest.Value);
    }

    [Fact]
    public async Task UpdateAsync_Valid_ReturnsUpdatedAccount()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updatedCommand = new CreateOrUpdateAccountCommand
        {
            Branch = "0002",
            AccountNumber = "65432-1",
            Balance = 3000.00m,
            CustomerId = Guid.NewGuid()
        };

        var updatedAccount = new Account(updatedCommand.Branch, updatedCommand.AccountNumber, updatedCommand.Balance, updatedCommand.CustomerId)
        {
            Id = id
        };

        _mockRepo.Setup(r => r.UpdateAsync(id, updatedCommand)).ReturnsAsync(updatedAccount);

        // Act
        var result = await _controller.UpdateAsync(id, updatedCommand);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(updatedAccount, createdResult.Value);
    }

    [Fact]
    public async Task UpdateAsync_InvalidOperation_ReturnsBadRequest()
    {
        var id = Guid.NewGuid();
        var updatedCommand = new CreateOrUpdateAccountCommand();

        _mockRepo.Setup(r => r.UpdateAsync(id, updatedCommand)).ThrowsAsync(new InvalidOperationException("Erro"));

        var result = await _controller.UpdateAsync(id, updatedCommand);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erro", badRequest.Value);
    }

    [Fact]
    public async Task DeleteAsync_ValidId_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteAsync(id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_InvalidOperation_ReturnsBadRequest()
    {
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new InvalidOperationException("Erro"));

        var result = await _controller.DeleteAsync(id);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Erro", badRequest.Value);
    }
}