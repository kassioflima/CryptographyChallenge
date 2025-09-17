using Cryptography.Domain.Interfaces;

namespace Cryptography.Domain.Entities
{
    public class CryptData
    {
        private readonly ICryptographyProvider? _cryptographyProvider;

        public long Id { get; set; }
        public string UserDocument { get; set; } = string.Empty;
        public string CreditCardToken { get; set; } = string.Empty;
        public long Value { get; set; }

        // Construtor para Entity Framework
        public CryptData() { }

        // Construtor para uso com criptografia
        public CryptData(ICryptographyProvider cryptographyProvider)
        {
            _cryptographyProvider = cryptographyProvider;
        }

        // Propriedades para acesso aos dados descriptografados
        public string DecryptedUserDocument =>
            _cryptographyProvider?.Decrypt(UserDocument) ?? UserDocument;

        public string DecryptedCreditCardToken =>
            _cryptographyProvider?.Decrypt(CreditCardToken) ?? CreditCardToken;

        // Métodos para definir dados criptografados
        public void SetUserDocument(string plainText)
        {
            if (_cryptographyProvider != null)
                UserDocument = _cryptographyProvider.Encrypt(plainText);
            else
                UserDocument = plainText;
        }

        public void SetCreditCardToken(string plainText)
        {
            if (_cryptographyProvider != null)
                CreditCardToken = _cryptographyProvider.Encrypt(plainText);
            else
                CreditCardToken = plainText;
        }

        // Método factory para criar instância com dados criptografados
        public static CryptData CreateWithEncryption(
            ICryptographyProvider cryptographyProvider,
            string userDocument,
            string creditCardToken,
            long value)
        {
            ArgumentNullException.ThrowIfNull(cryptographyProvider);
            
            var cryptData = new CryptData(cryptographyProvider)
            {
                Value = value
            };
            
            cryptData.SetUserDocument(userDocument);
            cryptData.SetCreditCardToken(creditCardToken);
            
            return cryptData;
        }
    }
}
