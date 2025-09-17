# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["Cryptography.API/Cryptography.API.csproj", "Cryptography.API/"]
COPY ["Cryptography.Domain/Cryptography.Domain.csproj", "Cryptography.Domain/"]
COPY ["Cryptography.Services/Cryptography.Services.csproj", "Cryptography.Services/"]
COPY ["Cryptography.Data/Cryptography.Data.csproj", "Cryptography.Data/"]

# Restore dependencies
RUN dotnet restore "Cryptography.API/Cryptography.API.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Cryptography.API"
RUN dotnet build "Cryptography.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Cryptography.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Cryptography.API.dll"]
