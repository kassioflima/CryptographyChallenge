using Cryptography.Domain.Entities;
using Cryptography.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Cryptography.Tests.Domain.Entities;

public class CryptDataTests
{
    private readonly Mock<ICryptographyProvider> _mockCryptographyProvider;

    public CryptDataTests()
    {
        _mockCryptographyProvider = new Mock<ICryptographyProvider>();
    }

    [Fact]
    public void Constructor_WithoutProvider_ShouldCreateInstance()
    {
        // Act
        var cryptData = new CryptData();

        // Assert
        cryptData.Should().NotBeNull();
        cryptData.Id.Should().Be(0);
        cryptData.UserDocument.Should().BeEmpty();
        cryptData.CreditCardToken.Should().BeEmpty();
        cryptData.Value.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithProvider_ShouldCreateInstance()
    {
        // Act
        var cryptData = new CryptData(_mockCryptographyProvider.Object);

        // Assert
        cryptData.Should().NotBeNull();
    }

    [Fact]
    public void SetUserDocument_WithProvider_ShouldEncryptData()
    {
        // Arrange
        const string plainText = "12345678901";
        const string encryptedText = "encrypted_user_document";
        _mockCryptographyProvider.Setup(x => x.Encrypt(plainText)).Returns(encryptedText);
        var cryptData = new CryptData(_mockCryptographyProvider.Object);

        // Act
        cryptData.SetUserDocument(plainText);

        // Assert
        cryptData.UserDocument.Should().Be(encryptedText);
        _mockCryptographyProvider.Verify(x => x.Encrypt(plainText), Times.Once);
    }

    [Fact]
    public void SetUserDocument_WithoutProvider_ShouldStorePlainText()
    {
        // Arrange
        const string plainText = "12345678901";
        var cryptData = new CryptData();

        // Act
        cryptData.SetUserDocument(plainText);

        // Assert
        cryptData.UserDocument.Should().Be(plainText);
    }

    [Fact]
    public void SetCreditCardToken_WithProvider_ShouldEncryptData()
    {
        // Arrange
        const string plainText = "1234567890123456";
        const string encryptedText = "encrypted_credit_card";
        _mockCryptographyProvider.Setup(x => x.Encrypt(plainText)).Returns(encryptedText);
        var cryptData = new CryptData(_mockCryptographyProvider.Object);

        // Act
        cryptData.SetCreditCardToken(plainText);

        // Assert
        cryptData.CreditCardToken.Should().Be(encryptedText);
        _mockCryptographyProvider.Verify(x => x.Encrypt(plainText), Times.Once);
    }

    [Fact]
    public void SetCreditCardToken_WithoutProvider_ShouldStorePlainText()
    {
        // Arrange
        const string plainText = "1234567890123456";
        var cryptData = new CryptData();

        // Act
        cryptData.SetCreditCardToken(plainText);

        // Assert
        cryptData.CreditCardToken.Should().Be(plainText);
    }

    [Fact]
    public void DecryptedUserDocument_WithProvider_ShouldDecryptData()
    {
        // Arrange
        const string encryptedText = "encrypted_user_document";
        const string decryptedText = "12345678901";
        _mockCryptographyProvider.Setup(x => x.Decrypt(encryptedText)).Returns(decryptedText);
        var cryptData = new CryptData(_mockCryptographyProvider.Object)
        {
            UserDocument = encryptedText
        };

        // Act
        var result = cryptData.DecryptedUserDocument;

        // Assert
        result.Should().Be(decryptedText);
        _mockCryptographyProvider.Verify(x => x.Decrypt(encryptedText), Times.Once);
    }

    [Fact]
    public void DecryptedUserDocument_WithoutProvider_ShouldReturnStoredValue()
    {
        // Arrange
        const string storedValue = "12345678901";
        var cryptData = new CryptData
        {
            UserDocument = storedValue
        };

        // Act
        var result = cryptData.DecryptedUserDocument;

        // Assert
        result.Should().Be(storedValue);
    }

    [Fact]
    public void DecryptedCreditCardToken_WithProvider_ShouldDecryptData()
    {
        // Arrange
        const string encryptedText = "encrypted_credit_card";
        const string decryptedText = "1234567890123456";
        _mockCryptographyProvider.Setup(x => x.Decrypt(encryptedText)).Returns(decryptedText);
        var cryptData = new CryptData(_mockCryptographyProvider.Object)
        {
            CreditCardToken = encryptedText
        };

        // Act
        var result = cryptData.DecryptedCreditCardToken;

        // Assert
        result.Should().Be(decryptedText);
        _mockCryptographyProvider.Verify(x => x.Decrypt(encryptedText), Times.Once);
    }

    [Fact]
    public void DecryptedCreditCardToken_WithoutProvider_ShouldReturnStoredValue()
    {
        // Arrange
        const string storedValue = "1234567890123456";
        var cryptData = new CryptData
        {
            CreditCardToken = storedValue
        };

        // Act
        var result = cryptData.DecryptedCreditCardToken;

        // Assert
        result.Should().Be(storedValue);
    }

    [Fact]
    public void CreateWithEncryption_ShouldCreateInstanceWithEncryptedData()
    {
        // Arrange
        const string userDocument = "12345678901";
        const string creditCardToken = "1234567890123456";
        const long value = 1000L;
        const string encryptedUserDoc = "encrypted_user_document";
        const string encryptedCreditCard = "encrypted_credit_card";

        _mockCryptographyProvider.Setup(x => x.Encrypt(userDocument)).Returns(encryptedUserDoc);
        _mockCryptographyProvider.Setup(x => x.Encrypt(creditCardToken)).Returns(encryptedCreditCard);

        // Act
        var result = CryptData.CreateWithEncryption(
            _mockCryptographyProvider.Object,
            userDocument,
            creditCardToken,
            value);

        // Assert
        result.Should().NotBeNull();
        result.UserDocument.Should().Be(encryptedUserDoc);
        result.CreditCardToken.Should().Be(encryptedCreditCard);
        result.Value.Should().Be(value);
        _mockCryptographyProvider.Verify(x => x.Encrypt(userDocument), Times.Once);
        _mockCryptographyProvider.Verify(x => x.Encrypt(creditCardToken), Times.Once);
    }

    [Fact]
    public void CreateWithEncryption_WithNullProvider_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => CryptData.CreateWithEncryption(
            null!,
            "12345678901",
            "1234567890123456",
            1000L);

        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("", "1234567890123456", 1000L)]
    [InlineData("12345678901", "", 1000L)]
    public void CreateWithEncryption_WithInvalidInputs_ShouldHandleGracefully(
        string userDocument, string creditCardToken, long value)
    {
        // Arrange
        _mockCryptographyProvider.Setup(x => x.Encrypt(It.IsAny<string>())).Returns("encrypted");

        // Act
        var result = CryptData.CreateWithEncryption(
            _mockCryptographyProvider.Object,
            userDocument,
            creditCardToken,
            value);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(value);
    }
}
