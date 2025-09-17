using Cryptography.Domain.DTOs;
using Cryptography.Domain.Entities;
using Cryptography.Domain.Interfaces;
using Cryptography.Services.Services;
using FluentAssertions;
using Moq;

namespace Cryptography.Tests.Services.Services;

public class CryptographyServiceTests
{
    private readonly Mock<IRepository<CryptData>> _mockRepository;
    private readonly Mock<ICryptographyProvider> _mockCryptographyProvider;
    private readonly CryptographyService _service;

    public CryptographyServiceTests()
    {
        _mockRepository = new Mock<IRepository<CryptData>>();
        _mockCryptographyProvider = new Mock<ICryptographyProvider>();
        _service = new CryptographyService(_mockRepository.Object, _mockCryptographyProvider.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidData_ShouldCreateAndReturnDTO()
    {
        // Arrange
        var dto = new CryptDataDTO
        {
            UserDocument = "12345678901",
            CreditCardToken = "1234567890123456",
            Value = 1000L
        };

        var createdEntity = new CryptData
        {
            Id = 1,
            UserDocument = "encrypted_user_doc",
            CreditCardToken = "encrypted_credit_card",
            Value = 1000L
        };

        _mockCryptographyProvider.Setup(x => x.Encrypt(dto.UserDocument)).Returns("encrypted_user_doc");
        _mockCryptographyProvider.Setup(x => x.Encrypt(dto.CreditCardToken)).Returns("encrypted_credit_card");
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<CryptData>())).ReturnsAsync(createdEntity);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.UserDocument.Should().Be("encrypted_user_doc"); // Should be encrypted in the returned DTO
        result.CreditCardToken.Should().Be("encrypted_credit_card"); // Should be encrypted in the returned DTO
        result.Value.Should().Be(1000L);

        _mockCryptographyProvider.Verify(x => x.Encrypt(dto.UserDocument), Times.Once);
        _mockCryptographyProvider.Verify(x => x.Encrypt(dto.CreditCardToken), Times.Once);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<CryptData>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnDTO()
    {
        // Arrange
        const long id = 1;
        var entity = new CryptData
        {
            Id = id,
            UserDocument = "encrypted_user_doc",
            CreditCardToken = "encrypted_credit_card",
            Value = 1000L
        };

        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);
        _mockCryptographyProvider.Setup(x => x.Decrypt("encrypted_user_doc")).Returns("12345678901");
        _mockCryptographyProvider.Setup(x => x.Decrypt("encrypted_credit_card")).Returns("1234567890123456");

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.UserDocument.Should().Be("12345678901");
        result.CreditCardToken.Should().Be("1234567890123456");
        result.Value.Should().Be(1000L);

        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
        _mockCryptographyProvider.Verify(x => x.Decrypt("encrypted_user_doc"), Times.Once);
        _mockCryptographyProvider.Verify(x => x.Decrypt("encrypted_credit_card"), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        const long id = 999;
        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((CryptData?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithData_ShouldReturnAllDTOs()
    {
        // Arrange
        var entities = new List<CryptData>
        {
            new() { Id = 1, UserDocument = "encrypted_doc1", CreditCardToken = "encrypted_card1", Value = 1000L },
            new() { Id = 2, UserDocument = "encrypted_doc2", CreditCardToken = "encrypted_card2", Value = 2000L }
        };

        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(entities);
        _mockCryptographyProvider.Setup(x => x.Decrypt("encrypted_doc1")).Returns("doc1");
        _mockCryptographyProvider.Setup(x => x.Decrypt("encrypted_card1")).Returns("card1");
        _mockCryptographyProvider.Setup(x => x.Decrypt("encrypted_doc2")).Returns("doc2");
        _mockCryptographyProvider.Setup(x => x.Decrypt("encrypted_card2")).Returns("card2");

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(dto => dto.Id == 1 && dto.UserDocument == "doc1");
        result.Should().Contain(dto => dto.Id == 2 && dto.UserDocument == "doc2");

        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CryptData>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingId_ShouldUpdateAndReturnDTO()
    {
        // Arrange
        const long id = 1;
        var existingEntity = new CryptData
        {
            Id = id,
            UserDocument = "old_encrypted_doc",
            CreditCardToken = "old_encrypted_card",
            Value = 1000L
        };

        var dto = new CryptDataDTO
        {
            UserDocument = "new_doc",
            CreditCardToken = "new_card",
            Value = 2000L
        };

        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(existingEntity);
        _mockCryptographyProvider.Setup(x => x.Encrypt("new_doc")).Returns("new_encrypted_doc");
        _mockCryptographyProvider.Setup(x => x.Encrypt("new_card")).Returns("new_encrypted_card");
        _mockCryptographyProvider.Setup(x => x.Decrypt("new_encrypted_doc")).Returns("new_doc");
        _mockCryptographyProvider.Setup(x => x.Decrypt("new_encrypted_card")).Returns("new_card");

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.UserDocument.Should().Be("new_doc");
        result.CreditCardToken.Should().Be("new_card");
        result.Value.Should().Be(2000L);

        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<CryptData>()), Times.Once);
        _mockCryptographyProvider.Verify(x => x.Encrypt("new_doc"), Times.Once);
        _mockCryptographyProvider.Verify(x => x.Encrypt("new_card"), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        const long id = 999;
        var dto = new CryptDataDTO
        {
            UserDocument = "new_doc",
            CreditCardToken = "new_card",
            Value = 2000L
        };

        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((CryptData?)null);

        // Act
        var result = await _service.UpdateAsync(id, dto);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<CryptData>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ShouldReturnTrue()
    {
        // Arrange
        const long id = 1;
        var entity = new CryptData
        {
            Id = id,
            UserDocument = "doc_to_delete",
            CreditCardToken = "card_to_delete",
            Value = 1000L
        };

        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
        _mockRepository.Verify(x => x.DeleteAsync(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ShouldReturnFalse()
    {
        // Arrange
        const long id = 999;
        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((CryptData?)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.GetByIdAsync(id), Times.Once);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<CryptData>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNullData_ShouldThrowException()
    {
        // Act & Assert
        var action = async () => await _service.CreateAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_WithNullData_ShouldThrowException()
    {
        // Arrange
        const long id = 1;
        var entity = new CryptData { Id = id };
        _mockRepository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act & Assert
        var action = async () => await _service.UpdateAsync(id, null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetByIdAsync_InvalidId_ShouldReturnNull(long invalidId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(invalidId)).ReturnsAsync((CryptData?)null);

        // Act
        var result = await _service.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }
}
