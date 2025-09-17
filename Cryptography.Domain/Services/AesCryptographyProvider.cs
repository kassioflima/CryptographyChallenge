using Cryptography.Domain.Interfaces;
using Cryptography.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Cryptography.Domain.Services
{
    public class AesCryptographyProvider : ICryptographyProvider
    {
        private readonly string _encryptionKey;
        private readonly string _initializationVector;

    public AesCryptographyProvider(IOptions<CryptographySettings> cryptographySettings)
    {
        ArgumentNullException.ThrowIfNull(cryptographySettings);
        
        _encryptionKey = cryptographySettings.Value.EncryptionKey;
        _initializationVector = cryptographySettings.Value.InitializationVector;
        
        if (string.IsNullOrEmpty(_encryptionKey) || _encryptionKey.Length != 32)
            throw new ArgumentException("Encryption key must be exactly 32 characters long", nameof(cryptographySettings));
            
        if (string.IsNullOrEmpty(_initializationVector) || _initializationVector.Length != 16)
            throw new ArgumentException("Initialization vector must be exactly 16 characters long", nameof(cryptographySettings));
    }

    public string Decrypt(string cipherText)
    {
        ArgumentNullException.ThrowIfNull(cipherText);
        
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(_initializationVector);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    public string Encrypt(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText);
        
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(_initializationVector);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
    }
}
