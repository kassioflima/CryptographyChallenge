# Cryptography Service - Docker Setup

Este projeto inclui configuração completa do Docker com SQL Server para facilitar o desenvolvimento e deploy.

## 🐳 Arquivos Docker

- `Dockerfile` - Configuração da imagem da API
- `docker-compose.yml` - Orquestração dos serviços
- `docker-compose.override.yml` - Configurações de desenvolvimento
- `.dockerignore` - Arquivos ignorados no build
- `docker-scripts.sh` - Scripts de automação
- `init-db/` - Scripts de inicialização do banco

## 🚀 Início Rápido

### Pré-requisitos
- Docker Desktop instalado
- Docker Compose v3.8+

### Comandos Básicos

```bash
# Construir e iniciar todos os serviços
docker-compose up --build

# Iniciar em background
docker-compose up -d

# Parar serviços
docker-compose down

# Ver logs
docker-compose logs -f
```

### Usando o Script de Automação

```bash
# Tornar o script executável (Linux/Mac)
chmod +x docker-scripts.sh

# Construir imagens
./docker-scripts.sh build

# Iniciar serviços
./docker-scripts.sh up

# Ver logs
./docker-scripts.sh logs

# Parar serviços
./docker-scripts.sh down

# Executar migrations
./docker-scripts.sh migrate

# Executar testes
./docker-scripts.sh test
```

## 📊 Serviços Incluídos

### 1. SQL Server 2022
- **Porta**: 1433
- **Usuário**: sa
- **Senha**: Cryptography@123
- **Banco**: CryptographyDb
- **Health Check**: Automático

### 2. Cryptography API
- **Porta**: 8080
- **Swagger**: http://localhost:8080/swagger
- **Health Check**: Automático
- **Dependências**: SQL Server

### 3. SQL Tools (Opcional)
- **Profile**: tools
- **Uso**: `docker-compose --profile tools up`

## 🔧 Configurações

### Variáveis de Ambiente

#### SQL Server
```yaml
ACCEPT_EULA: Y
SA_PASSWORD: Cryptography@123
MSSQL_PID: Express
```

#### API
```yaml
ASPNETCORE_ENVIRONMENT: Production
ASPNETCORE_URLS: http://+:8080
ConnectionStrings__ConnectionString: Data Source=sqlserver,1433;...
```

### Volumes
- `sqlserver_data`: Dados persistentes do SQL Server
- `init-db`: Scripts de inicialização

### Networks
- `cryptography-network`: Rede isolada para comunicação entre serviços

## 🗄️ Banco de Dados

### Inicialização Automática
O banco é criado automaticamente com:
- Database: `CryptographyDb`
- Tabela: `CryptData`
- Dados de exemplo (opcional)

### Script de Inicialização
```sql
-- Arquivo: init-db/01-init-database.sql
CREATE DATABASE CryptographyDb;
USE CryptographyDb;
CREATE TABLE CryptData (...);
```

### Migrations
```bash
# Executar migrations
docker-compose exec cryptography-api dotnet ef database update

# Ou usando o script
./docker-scripts.sh migrate
```

## 🧪 Testes

### Executar Testes
```bash
# Todos os testes
docker-compose exec cryptography-api dotnet test

# Testes específicos
docker-compose exec cryptography-api dotnet test --filter "CryptDataTests"

# Com cobertura
docker-compose exec cryptography-api dotnet test --collect:"XPlat Code Coverage"
```

## 🔍 Debugging

### Logs
```bash
# Todos os serviços
docker-compose logs -f

# Serviço específico
docker-compose logs -f cryptography-api
docker-compose logs -f sqlserver
```

### Shell Access
```bash
# Shell na API
docker-compose exec cryptography-api /bin/bash

# SQL Shell
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Cryptography@123
```

### Health Checks
```bash
# Verificar status
docker-compose ps

# Health check da API
curl http://localhost:8080/swagger

# Health check do SQL Server
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Cryptography@123 -Q "SELECT 1"
```

## 🚀 Deploy em Produção

### Configurações de Produção
1. Alterar senhas padrão
2. Configurar SSL/TLS
3. Ajustar recursos (CPU/Memory)
4. Configurar backup do banco

### Exemplo de Deploy
```bash
# Build para produção
docker-compose -f docker-compose.yml -f docker-compose.prod.yml build

# Deploy
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## 📝 Comandos Úteis

```bash
# Rebuild sem cache
docker-compose build --no-cache

# Limpar tudo
docker-compose down -v --rmi all
docker system prune -f

# Verificar recursos
docker stats

# Backup do banco
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Cryptography@123 -Q "BACKUP DATABASE CryptographyDb TO DISK = '/var/opt/mssql/backup/CryptographyDb.bak'"
```

## 🔒 Segurança

### Recomendações
1. **Alterar senhas padrão** em produção
2. **Usar secrets** para dados sensíveis
3. **Configurar SSL/TLS** para comunicação
4. **Limitar acesso** às portas expostas
5. **Configurar firewall** adequadamente

### Exemplo com Secrets
```yaml
# docker-compose.prod.yml
services:
  sqlserver:
    environment:
      - SA_PASSWORD_FILE=/run/secrets/sa_password
    secrets:
      - sa_password
      
secrets:
  sa_password:
    file: ./secrets/sa_password.txt
```

## 🆘 Troubleshooting

### Problemas Comuns

1. **SQL Server não inicia**
   ```bash
   # Verificar logs
   docker-compose logs sqlserver
   
   # Verificar recursos
   docker stats
   ```

2. **API não conecta ao banco**
   ```bash
   # Verificar se SQL Server está saudável
   docker-compose ps
   
   # Testar conexão
   docker-compose exec cryptography-api dotnet ef database update
   ```

3. **Porta já em uso**
   ```bash
   # Verificar portas em uso
   netstat -tulpn | grep :1433
   netstat -tulpn | grep :8080
   
   # Alterar portas no docker-compose.yml
   ```

4. **Volumes corrompidos**
   ```bash
   # Remover volumes
   docker-compose down -v
   
   # Recriar
   docker-compose up -d
   ```

## 📚 Recursos Adicionais

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [SQL Server Docker](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment)
- [ASP.NET Core Docker](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
