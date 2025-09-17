using Cryptography.Domain.DTOs;
using Cryptography.Domain.Entities;
using Cryptography.Domain.Interfaces;
using Cryptography.Services.Interfaces;

namespace Cryptography.Services.Services
{
    public class CryptographyService : ICryptographyService
    {
        private readonly IRepository<CryptData> _repository;
        private readonly ICryptographyProvider _cryptographyProvider;

        public CryptographyService(IRepository<CryptData> repository, ICryptographyProvider cryptographyProvider)
        {
            _repository = repository;
            _cryptographyProvider = cryptographyProvider;
        }

    public async Task<CryptDataDTO> CreateAsync(CryptDataDTO data)
    {
        ArgumentNullException.ThrowIfNull(data);
        
        var entity = CryptData.CreateWithEncryption(
            _cryptographyProvider,
            data.UserDocument,
            data.CreditCardToken,
            data.Value);

        var createdEntity = await _repository.AddAsync(entity);
        return CryptDataDTO.FromEntity(createdEntity);
    }

        public async Task<CryptDataDTO?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            // Criar uma nova instância com o provider de criptografia para descriptografar
            var entityWithCrypto = new CryptData(_cryptographyProvider)
            {
                Id = entity.Id,
                UserDocument = entity.UserDocument,
                CreditCardToken = entity.CreditCardToken,
                Value = entity.Value
            };

            return CryptDataDTO.FromEntity(entityWithCrypto);
        }

        public async Task<IEnumerable<CryptDataDTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(entity =>
            {
                var entityWithCrypto = new CryptData(_cryptographyProvider)
                {
                    Id = entity.Id,
                    UserDocument = entity.UserDocument,
                    CreditCardToken = entity.CreditCardToken,
                    Value = entity.Value
                };
                return CryptDataDTO.FromEntity(entityWithCrypto);
            });
        }

    public async Task<CryptDataDTO?> UpdateAsync(long id, CryptDataDTO data)
    {
        ArgumentNullException.ThrowIfNull(data);
        
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return null;

        // Criar nova instância com criptografia para atualizar
        var entityWithCrypto = new CryptData(_cryptographyProvider)
        {
            Id = entity.Id,
            UserDocument = entity.UserDocument,
            CreditCardToken = entity.CreditCardToken,
            Value = entity.Value
        };

        entityWithCrypto.SetUserDocument(data.UserDocument);
        entityWithCrypto.SetCreditCardToken(data.CreditCardToken);
        entityWithCrypto.Value = data.Value;

        // Atualizar a entidade original com os dados criptografados
        entity.UserDocument = entityWithCrypto.UserDocument;
        entity.CreditCardToken = entityWithCrypto.CreditCardToken;
        entity.Value = entityWithCrypto.Value;

        await _repository.UpdateAsync(entity);
        return CryptDataDTO.FromEntity(entityWithCrypto);
    }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            await _repository.DeleteAsync(entity);
            return true;
        }
    }
}