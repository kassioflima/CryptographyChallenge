using Cryptography.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cryptography.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240504194554_CreateDb")]
    partial class CreateDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Cryptography.Domain.Entities.CryptData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreditCardToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserDocument")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("CryptData");
                });
#pragma warning restore 612, 618
        }
    }
}
