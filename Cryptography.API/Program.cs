using Cryptography.Data;
using Cryptography.Data.Repositories;
using Cryptography.Domain.Entities;
using Cryptography.Domain.Interfaces;
using Cryptography.Domain.Services;
using Cryptography.Domain.Settings;
using Cryptography.Services.Interfaces;
using Cryptography.Services.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseSqlServer(connectionString);
});

// Configurações de criptografia
builder.Services.Configure<CryptographySettings>(builder.Configuration.GetSection("CryptographySettings"));

// Registro do provider de criptografia
builder.Services.AddScoped<ICryptographyProvider, AesCryptographyProvider>();

// Registro do repositório
builder.Services.AddScoped<IRepository<CryptData>, CryptDataRepository>();

// Registro do serviço de aplicação
builder.Services.AddScoped<ICryptographyService, CryptographyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
