using Cryptography.Data;
using Cryptography.Data.Repositories;
using Cryptography.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Cryptography.Tests.Data.Repositories;

public class CryptDataRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CryptDataRepository _repository;

    public CryptDataRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CryptDataRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ValidEntity_ShouldAddToDatabase()
    {
        // Arrange
        var entity = new CryptData
        {
            UserDocument = "encrypted_user_doc",
            CreditCardToken = "encrypted_credit_card",
            Value = 1000L
        };

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.UserDocument.Should().Be(entity.UserDocument);
        result.CreditCardToken.Should().Be(entity.CreditCardToken);
        result.Value.Should().Be(entity.Value);

        // Verify in database
        var savedEntity = await _context.CryptData.FindAsync(result.Id);
        savedEntity.Should().NotBeNull();
        savedEntity!.UserDocument.Should().Be(entity.UserDocument);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ShouldReturnEntity()
    {
        // Arrange
        var entity = new CryptData
        {
            UserDocument = "encrypted_user_doc",
            CreditCardToken = "encrypted_credit_card",
            Value = 1000L
        };
        await _context.CryptData.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(entity.Id);
        result.UserDocument.Should().Be(entity.UserDocument);
        result.CreditCardToken.Should().Be(entity.CreditCardToken);
        result.Value.Should().Be(entity.Value);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        const long nonExistingId = 999L;

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithData_ShouldReturnAllEntities()
    {
        // Arrange
        var entities = new List<CryptData>
        {
            new() { UserDocument = "doc1", CreditCardToken = "card1", Value = 1000L },
            new() { UserDocument = "doc2", CreditCardToken = "card2", Value = 2000L },
            new() { UserDocument = "doc3", CreditCardToken = "card3", Value = 3000L }
        };

        await _context.CryptData.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(e => e.UserDocument == "doc1");
        result.Should().Contain(e => e.UserDocument == "doc2");
        result.Should().Contain(e => e.UserDocument == "doc3");
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ShouldReturnEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ExistingEntity_ShouldUpdateInDatabase()
    {
        // Arrange
        var entity = new CryptData
        {
            UserDocument = "original_doc",
            CreditCardToken = "original_card",
            Value = 1000L
        };
        await _context.CryptData.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.UserDocument = "updated_doc";
        entity.CreditCardToken = "updated_card";
        entity.Value = 2000L;
        await _repository.UpdateAsync(entity);

        // Assert
        var updatedEntity = await _context.CryptData.FindAsync(entity.Id);
        updatedEntity.Should().NotBeNull();
        updatedEntity!.UserDocument.Should().Be("updated_doc");
        updatedEntity.CreditCardToken.Should().Be("updated_card");
        updatedEntity.Value.Should().Be(2000L);
    }

    [Fact]
    public async Task DeleteAsync_ExistingEntity_ShouldRemoveFromDatabase()
    {
        // Arrange
        var entity = new CryptData
        {
            UserDocument = "doc_to_delete",
            CreditCardToken = "card_to_delete",
            Value = 1000L
        };
        await _context.CryptData.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(entity);

        // Assert
        var deletedEntity = await _context.CryptData.FindAsync(entity.Id);
        deletedEntity.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistChanges()
    {
        // Arrange
        var entity = new CryptData
        {
            UserDocument = "test_doc",
            CreditCardToken = "test_card",
            Value = 1000L
        };
        await _context.CryptData.AddAsync(entity);

        // Act
        await _repository.SaveAsync();

        // Assert
        var savedEntity = await _context.CryptData.FindAsync(entity.Id);
        savedEntity.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_MultipleEntities_ShouldAssignUniqueIds()
    {
        // Arrange
        var entity1 = new CryptData
        {
            UserDocument = "doc1",
            CreditCardToken = "card1",
            Value = 1000L
        };
        var entity2 = new CryptData
        {
            UserDocument = "doc2",
            CreditCardToken = "card2",
            Value = 2000L
        };

        // Act
        var result1 = await _repository.AddAsync(entity1);
        var result2 = await _repository.AddAsync(entity2);

        // Assert
        result1.Id.Should().NotBe(result2.Id);
        result1.Id.Should().BeGreaterThan(0);
        result2.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdAsync_WithDifferentIds_ShouldReturnCorrectEntity()
    {
        // Arrange
        var entities = new List<CryptData>
        {
            new() { UserDocument = "doc1", CreditCardToken = "card1", Value = 1000L },
            new() { UserDocument = "doc2", CreditCardToken = "card2", Value = 2000L },
            new() { UserDocument = "doc3", CreditCardToken = "card3", Value = 3000L }
        };

        await _context.CryptData.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var result1 = await _repository.GetByIdAsync(entities[0].Id);
        var result2 = await _repository.GetByIdAsync(entities[1].Id);
        var result3 = await _repository.GetByIdAsync(entities[2].Id);

        // Assert
        result1!.UserDocument.Should().Be("doc1");
        result2!.UserDocument.Should().Be("doc2");
        result3!.UserDocument.Should().Be("doc3");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
