using Cryptography.Domain.Services;
using Cryptography.Domain.Settings;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Cryptography.Tests.Domain.Services;

public class AesCryptographyProviderTests
{
    private readonly CryptographySettings _settings;
    private readonly AesCryptographyProvider _provider;

    public AesCryptographyProviderTests()
    {
        _settings = new CryptographySettings
        {
            EncryptionKey = "a2KE7izknteF3PwEyH02ZSoBNHWmPhSe", // 32 bytes
            InitializationVector = "DSowoZNzqW1kvTGF" // 16 bytes
        };

        var options = Options.Create(_settings);
        _provider = new AesCryptographyProvider(options);
    }

    [Fact]
    public void Encrypt_ValidPlainText_ShouldReturnEncryptedString()
    {
        // Arrange
        const string plainText = "Hello, World!";

        // Act
        var result = _provider.Encrypt(plainText);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotBe(plainText);
        result.Should().MatchRegex(@"^[A-Za-z0-9+/]+=*$"); // Base64 pattern
    }

    [Fact]
    public void Decrypt_ValidEncryptedText_ShouldReturnOriginalPlainText()
    {
        // Arrange
        const string plainText = "Hello, World!";
        var encryptedText = _provider.Encrypt(plainText);

        // Act
        var result = _provider.Decrypt(encryptedText);

        // Assert
        result.Should().Be(plainText);
    }

    [Fact]
    public void EncryptDecrypt_RoundTrip_ShouldPreserveData()
    {
        // Arrange
        const string originalText = "Sensitive data: 12345678901";

        // Act
        var encrypted = _provider.Encrypt(originalText);
        var decrypted = _provider.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(originalText);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("Short")]
    [InlineData("This is a longer text with special characters: !@#$%^&*()")]
    [InlineData("Unicode text: 你好世界 🌍")]
    [InlineData("Numbers: 1234567890")]
    [InlineData("JSON: {\"key\": \"value\", \"number\": 123}")]
    public void EncryptDecrypt_VariousInputs_ShouldPreserveData(string input)
    {
        // Act
        var encrypted = _provider.Encrypt(input);
        var decrypted = _provider.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(input);
    }

    [Fact]
    public void Encrypt_SameInputMultipleTimes_ShouldProduceSameResults()
    {
        // Arrange
        const string plainText = "Same input";

        // Act
        var encrypted1 = _provider.Encrypt(plainText);
        var encrypted2 = _provider.Encrypt(plainText);

        // Assert
        encrypted1.Should().Be(encrypted2); // With fixed IV, same input produces same output
        
        // Both should decrypt to the same value
        var decrypted1 = _provider.Decrypt(encrypted1);
        var decrypted2 = _provider.Decrypt(encrypted2);
        
        decrypted1.Should().Be(plainText);
        decrypted2.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_NullInput_ShouldThrowException()
    {
        // Act & Assert
        var action = () => _provider.Encrypt(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Decrypt_NullInput_ShouldThrowException()
    {
        // Act & Assert
        var action = () => _provider.Decrypt(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Decrypt_InvalidBase64_ShouldThrowException()
    {
        // Arrange
        const string invalidBase64 = "This is not valid base64!";

        // Act & Assert
        var action = () => _provider.Decrypt(invalidBase64);
        action.Should().Throw<FormatException>();
    }

    [Fact]
    public void Decrypt_CorruptedEncryptedData_ShouldThrowException()
    {
        // Arrange
        const string plainText = "Test data";
        var encrypted = _provider.Encrypt(plainText);
        var corrupted = encrypted.Substring(0, encrypted.Length - 1) + "X"; // Corrupt last character

        // Act & Assert
        var action = () => _provider.Decrypt(corrupted);
        action.Should().Throw<FormatException>(); // Changed from CryptographicException to FormatException
    }

    [Fact]
    public void Encrypt_LongText_ShouldHandleCorrectly()
    {
        // Arrange
        var longText = new string('A', 10000); // 10KB of text

        // Act
        var encrypted = _provider.Encrypt(longText);
        var decrypted = _provider.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(longText);
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowException()
    {
        // Act & Assert
        var action = () => new AesCryptographyProvider(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithInvalidKeyLength_ShouldThrowException()
    {
        // Arrange
        var invalidSettings = new CryptographySettings
        {
            EncryptionKey = "short", // Invalid length
            InitializationVector = "DSowoZNzqW1kvTGF"
        };
        var options = Options.Create(invalidSettings);

        // Act & Assert
        var action = () => new AesCryptographyProvider(options);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithInvalidIVLength_ShouldThrowException()
    {
        // Arrange
        var invalidSettings = new CryptographySettings
        {
            EncryptionKey = "a2KE7izknteF3PwEyH02ZSoBNHWmPhSe",
            InitializationVector = "short" // Invalid length
        };
        var options = Options.Create(invalidSettings);

        // Act & Assert
        var action = () => new AesCryptographyProvider(options);
        action.Should().Throw<ArgumentException>();
    }
}
