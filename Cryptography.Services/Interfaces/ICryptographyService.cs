using Cryptography.Domain.DTOs;

namespace Cryptography.Services.Interfaces
{
    public interface ICryptographyService
    {
        Task<CryptDataDTO> CreateAsync(CryptDataDTO data);
        Task<CryptDataDTO?> GetByIdAsync(long id);
        Task<IEnumerable<CryptDataDTO>> GetAllAsync();
        Task<CryptDataDTO?> UpdateAsync(long id, CryptDataDTO data);
        Task<bool> DeleteAsync(long id);
    }
}
