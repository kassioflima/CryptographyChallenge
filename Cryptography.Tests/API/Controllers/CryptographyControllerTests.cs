using Cryptography.API.Controllers;
using Cryptography.Domain.DTOs;
using Cryptography.Domain.Entities;
using Cryptography.Domain.Interfaces;
using Cryptography.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cryptography.Tests.API.Controllers;

public class CryptographyControllerTests
{
    private readonly Mock<ICryptographyService> _mockService;
    private readonly Mock<IRepository<CryptData>> _mockRepository;
    private readonly Mock<ICryptographyProvider> _mockCryptographyProvider;
    private readonly CryptographyController _controller;

    public CryptographyControllerTests()
    {
        _mockService = new Mock<ICryptographyService>();
        _mockRepository = new Mock<IRepository<CryptData>>();
        _mockCryptographyProvider = new Mock<ICryptographyProvider>();
        
        _controller = new CryptographyController(
            _mockService.Object,
            _mockRepository.Object,
            _mockCryptographyProvider.Object);
    }

    [Fact]
    public async Task GetAll_EncryptedFalse_ShouldReturnDecryptedData()
    {
        // Arrange
        var dtos = new List<CryptDataDTO>
        {
            new() { Id = 1, UserDocument = "doc1", CreditCardToken = "card1", Value = 1000L },
            new() { Id = 2, UserDocument = "doc2", CreditCardToken = "card2", Value = 2000L }
        };

        _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(dtos);

        // Act
        var result = await _controller.GetAll(false);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(dtos);
        _mockService.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAll_EncryptedTrue_ShouldReturnEncryptedData()
    {
        // Arrange
        var entities = new List<CryptData>
        {
            new() { Id = 1, UserDocument = "encrypted_doc1", CreditCardToken = "encrypted_card1", Value = 1000L },
            new() { Id = 2, UserDocument = "encrypted_doc2", CreditCardToken = "encrypted_card2", Value = 2000L }
        };

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(entities);

        // Act
        var result = await _controller.GetAll(true);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(entities);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetById_EncryptedFalse_ExistingId_ShouldReturnDecryptedData()
    {
        // Arrange
        const long id = 1;
        var dto = new CryptDataDTO
        {
            Id = id,
            UserDocument = "doc1",
            CreditCardToken = "card1",
            Value = 1000L
        };

        _mockService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetById(id, false);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(dto);
        _mockService.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetById_EncryptedTrue_ExistingId_ShouldReturnEncryptedData()
    {
        // Arrange
        const long id = 1;
        var entity = new CryptData
        {
            Id = id,
            UserDocument = "encrypted_doc1",
            CreditCardToken = "encrypted_card1",
            Value = 1000L
        };

        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act
        var result = await _controller.GetById(id, true);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(entity);
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetById_EncryptedFalse_NonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        const long id = 999;
        _mockService.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((CryptDataDTO?)null);

        // Act
        var result = await _controller.GetById(id, false);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetById_EncryptedTrue_NonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        const long id = 999;
        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((CryptData?)null);

        // Act
        var result = await _controller.GetById(id, true);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task Create_ValidData_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var dto = new CryptDataDTO
        {
            UserDocument = "12345678901",
            CreditCardToken = "1234567890123456",
            Value = 1000L
        };

        var createdDto = new CryptDataDTO
        {
            Id = 1,
            UserDocument = "12345678901",
            CreditCardToken = "1234567890123456",
            Value = 1000L
        };

        _mockService.Setup(x => x.CreateAsync(dto)).ReturnsAsync(createdDto);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(_controller.GetById));
        createdResult.RouteValues!["id"].Should().Be(1);
        createdResult.RouteValues["encrypted"].Should().Be(true);
        createdResult.Value.Should().BeEquivalentTo(createdDto);
        _mockService.Verify(x => x.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Update_ExistingId_ShouldReturnNoContent()
    {
        // Arrange
        const long id = 1;
        var dto = new CryptDataDTO
        {
            UserDocument = "updated_doc",
            CreditCardToken = "updated_card",
            Value = 2000L
        };

        var updatedDto = new CryptDataDTO
        {
            Id = id,
            UserDocument = "updated_doc",
            CreditCardToken = "updated_card",
            Value = 2000L
        };

        _mockService.Setup(x => x.UpdateAsync(id, dto)).ReturnsAsync(updatedDto);

        // Act
        var result = await _controller.Update(id, dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(x => x.UpdateAsync(id, dto), Times.Once);
    }

    [Fact]
    public async Task Update_NonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        const long id = 999;
        var dto = new CryptDataDTO
        {
            UserDocument = "updated_doc",
            CreditCardToken = "updated_card",
            Value = 2000L
        };

        _mockService.Setup(x => x.UpdateAsync(id, dto)).ReturnsAsync((CryptDataDTO?)null);

        // Act
        var result = await _controller.Update(id, dto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(x => x.UpdateAsync(id, dto), Times.Once);
    }

    [Fact]
    public async Task DeleteById_ExistingId_ShouldReturnNoContent()
    {
        // Arrange
        const long id = 1;
        _mockService.Setup(x => x.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteById(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteById_NonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        const long id = 999;
        _mockService.Setup(x => x.DeleteAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteById(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockService.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task Create_WithNullData_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.Create(null!);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Update_WithNullData_ShouldReturnBadRequest()
    {
        // Arrange
        const long id = 1;

        // Act
        var result = await _controller.Update(id, null!);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetById_InvalidId_ShouldReturnNotFound(long invalidId)
    {
        // Arrange
        _mockService.Setup(x => x.GetByIdAsync(invalidId)).ReturnsAsync((CryptDataDTO?)null);

        // Act
        var result = await _controller.GetById(invalidId, false);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Update_InvalidId_ShouldReturnNotFound(long invalidId)
    {
        // Arrange
        var dto = new CryptDataDTO
        {
            UserDocument = "doc",
            CreditCardToken = "card",
            Value = 1000L
        };

        _mockService.Setup(x => x.UpdateAsync(invalidId, dto)).ReturnsAsync((CryptDataDTO?)null);

        // Act
        var result = await _controller.Update(invalidId, dto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteById_InvalidId_ShouldReturnNotFound(long invalidId)
    {
        // Arrange
        _mockService.Setup(x => x.DeleteAsync(invalidId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteById(invalidId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
