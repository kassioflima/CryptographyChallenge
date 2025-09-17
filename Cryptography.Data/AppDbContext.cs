using Cryptography.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cryptography.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CryptData> CryptData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CryptData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserDocument).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreditCardToken).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Value).IsRequired();
            });
        }
    }
}
