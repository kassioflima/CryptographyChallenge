namespace Cryptography.Domain.Settings
{
    public class CryptographySettings
    {
        public string EncryptionKey { get; set; } = string.Empty;
        public string InitializationVector { get; set; } = string.Empty;
    }
}
