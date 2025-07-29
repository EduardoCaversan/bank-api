using Moq;
using Microsoft.AspNetCore.Mvc;
using BankApp.WebApi.Controllers;
using BankApp.Application.Interfaces;
using BankApp.Domain.Entities;
using BankApp.Domain.DTOs;

namespace BankApp.Tests.Controllers
{
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
        public async Task Get_ReturnsOk_WithPaginatedAccounts()
        {
            // Arrange
            var accounts = new List<Account> { new() { Id = Guid.NewGuid(), Branch = "0001" } };

            var paginatedResponse = new PaginatedResponse<Account>
            {
                Data = accounts,
                Pagination = new PaginationMetadata
                {
                    ItemsPerPage = 10,
                    CurrentPage = 1,
                    TotalItems = 1,
                    TotalPages = 1
                }
            };

            _mockRepo.Setup(r => r.GetAllPaginatedAsync(10, 1)).ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<PaginatedResponse<Account>>(okResult.Value);
            Assert.Equal(paginatedResponse.Data, returnedValue.Data);
            Assert.Equal(paginatedResponse.Pagination.TotalItems, returnedValue.Pagination.TotalItems);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var account = new Account { Id = Guid.NewGuid(), Branch = "0001" };
            _mockRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

            var result = await _controller.GetById(account.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(account, okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Account?)null);

            var result = await _controller.GetById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenValid()
        {
            var account = new Account { Id = Guid.NewGuid(), Branch = "0001" };

            _mockRepo.Setup(r => r.AddAsync(account)).Returns(Task.CompletedTask);

            var result = await _controller.CreateAsync(account);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(account, created.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_OnException()
        {
            var account = new Account { Id = Guid.NewGuid(), Branch = "0001" };

            _mockRepo.Setup(r => r.AddAsync(account))
                     .ThrowsAsync(new InvalidOperationException("Conta duplicada"));

            var result = await _controller.CreateAsync(account);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Conta duplicada", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            var account = new Account { Id = Guid.NewGuid() };

            var result = await _controller.UpdateAsync(Guid.NewGuid(), account);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID do corpo não corresponde ao ID da URL.", badRequest.Value);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            var account = new Account { Id = Guid.NewGuid(), Branch = "0001" };

            _mockRepo.Setup(r => r.UpdateAsync(account)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateAsync(account.Id, account);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_OnException()
        {
            var account = new Account { Id = Guid.NewGuid(), Branch = "0001" };

            _mockRepo.Setup(r => r.UpdateAsync(account))
                     .ThrowsAsync(new InvalidOperationException("Erro de update"));

            var result = await _controller.UpdateAsync(account.Id, account);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro de update", badRequest.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAccountNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((Account?)null);

            var result = await _controller.DeleteAsync(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            var account = new Account { Id = Guid.NewGuid(), Branch = "0001" };

            _mockRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);
            _mockRepo.Setup(r => r.DeleteAsync(account)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteAsync(account.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_OnException()
        {
            var account = new Account { Id = Guid.NewGuid(), Branch = "0001" };

            _mockRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);
            _mockRepo.Setup(r => r.DeleteAsync(account))
                     .ThrowsAsync(new InvalidOperationException("Erro ao deletar"));

            var result = await _controller.DeleteAsync(account.Id);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro ao deletar", badRequest.Value);
        }
    }
}