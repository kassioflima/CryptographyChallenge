using Cryptography.Domain.Entities;

namespace Cryptography.Domain.DTOs
{
    public record CryptDataDTO
    {
        public long Id { get; init; }
        public string UserDocument { get; init; } = string.Empty;
        public string CreditCardToken { get; init; } = string.Empty;
        public long Value { get; init; }

        public static CryptDataDTO FromEntity(CryptData entity)
        {
            return new CryptDataDTO
            {
                Id = entity.Id,
                UserDocument = entity.DecryptedUserDocument,
                CreditCardToken = entity.DecryptedCreditCardToken,
                Value = entity.Value
            };
        }
    }
}
