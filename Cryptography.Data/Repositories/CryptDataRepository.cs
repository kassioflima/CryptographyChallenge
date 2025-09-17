using Cryptography.Domain.Entities;
using Cryptography.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cryptography.Data.Repositories
{
    public class CryptDataRepository : IRepository<CryptData>
    {
        private readonly AppDbContext _context;

        public CryptDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CryptData> AddAsync(CryptData entity)
        {
            var createdEntity = await _context.CryptData.AddAsync(entity);
            await SaveAsync();
            return createdEntity.Entity;
        }

        public async Task DeleteAsync(CryptData entity)
        {
            _context.CryptData.Remove(entity);
            await SaveAsync();
        }

        public async Task<IEnumerable<CryptData>> GetAllAsync()
        {
            return await _context.CryptData.ToListAsync();
        }

        public Task<CryptData?> GetByIdAsync(long id)
        {
            return _context.CryptData.FirstOrDefaultAsync(cp => cp.Id == id);
        }

        public async Task UpdateAsync(CryptData entity)
        {
            _context.CryptData.Update(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
