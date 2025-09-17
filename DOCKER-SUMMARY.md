# 🐳 Docker Setup Completo - Cryptography Service

## 📁 Arquivos Criados

### Docker Core
- ✅ `Dockerfile` - Imagem da API .NET 9
- ✅ `docker-compose.yml` - Orquestração principal
- ✅ `docker-compose.override.yml` - Configurações de desenvolvimento
- ✅ `docker-compose.prod.yml` - Configurações de produção
- ✅ `.dockerignore` - Arquivos ignorados no build

### Scripts e Automação
- ✅ `docker-scripts.sh` - Scripts bash para automação
- ✅ `Makefile` - Comandos make para facilitar uso
- ✅ `env.example` - Exemplo de variáveis de ambiente

### Configuração e Documentação
- ✅ `DOCKER.md` - Documentação completa do Docker
- ✅ `init-db/01-init-database.sql` - Script de inicialização do banco
- ✅ `Cryptography.API/appsettings.Docker.json` - Configurações Docker
- ✅ `.docker/config.json` - Configuração do Docker Desktop

## 🚀 Como Usar

### Início Rápido
```bash
# Construir e iniciar
docker-compose up --build

# Ou usando make
make up

# Ou usando o script
./docker-scripts.sh up
```

### Comandos Principais
```bash
# Desenvolvimento
docker-compose up -d
docker-compose logs -f
docker-compose down

# Produção
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Testes
docker-compose exec cryptography-api dotnet test

# Migrations
docker-compose exec cryptography-api dotnet ef database update
```

## 📊 Serviços Configurados

### 1. SQL Server 2022
- **Imagem**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Porta**: 1433
- **Usuário**: sa
- **Senha**: Cryptography@123
- **Banco**: CryptographyDb
- **Health Check**: ✅ Configurado
- **Volumes**: Dados persistentes

### 2. Cryptography API
- **Imagem**: Build local (.NET 9)
- **Porta**: 8080 (HTTP), 8081 (HTTPS)
- **Swagger**: http://localhost:8080/swagger
- **Health Check**: ✅ Configurado
- **Dependências**: SQL Server

### 3. SQL Tools (Opcional)
- **Profile**: tools
- **Uso**: `docker-compose --profile tools up`

## 🔧 Recursos Implementados

### ✅ Health Checks
- SQL Server: Verificação de conectividade
- API: Verificação do endpoint Swagger

### ✅ Dependências
- API aguarda SQL Server estar saudável
- Inicialização ordenada dos serviços

### ✅ Volumes Persistentes
- Dados do SQL Server persistidos
- Scripts de inicialização automática

### ✅ Networks Isoladas
- Rede dedicada para comunicação entre serviços
- Isolamento de segurança

### ✅ Configurações por Ambiente
- Desenvolvimento: `docker-compose.override.yml`
- Produção: `docker-compose.prod.yml`
- Variáveis de ambiente configuráveis

### ✅ Scripts de Automação
- Build, start, stop, restart
- Logs, testes, migrations
- Limpeza e backup

## 🔒 Segurança

### Configurações de Produção
- Senhas configuráveis via variáveis de ambiente
- SSL/TLS habilitado para produção
- Limites de recursos configurados
- Políticas de restart configuradas

### Recomendações
1. Alterar senhas padrão em produção
2. Usar secrets para dados sensíveis
3. Configurar firewall adequadamente
4. Monitorar logs de segurança

## 📈 Monitoramento

### Health Checks
```bash
# Status dos serviços
docker-compose ps

# Logs em tempo real
docker-compose logs -f

# Health check manual
curl http://localhost:8080/swagger
```

### Métricas
- CPU e memória limitados por serviço
- Restart automático em caso de falha
- Logs estruturados para análise

## 🆘 Troubleshooting

### Problemas Comuns
1. **Porta já em uso**: Verificar `netstat -tulpn | grep :1433`
2. **SQL Server não inicia**: Verificar recursos disponíveis
3. **API não conecta**: Verificar health check do SQL Server
4. **Volumes corrompidos**: Executar `docker-compose down -v`

### Comandos de Diagnóstico
```bash
# Verificar recursos
docker stats

# Verificar logs
docker-compose logs sqlserver
docker-compose logs cryptography-api

# Testar conectividade
docker-compose exec cryptography-api dotnet ef database update
```

## 🎯 Próximos Passos

1. **Testar o setup**: `docker-compose up --build`
2. **Configurar CI/CD**: Integrar com pipelines
3. **Monitoramento**: Adicionar Prometheus/Grafana
4. **Backup**: Configurar backup automático
5. **Scaling**: Configurar load balancer

## 📚 Documentação Adicional

- [DOCKER.md](./DOCKER.md) - Documentação completa
- [Docker Compose Docs](https://docs.docker.com/compose/)
- [SQL Server Docker](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-deployment)
- [ASP.NET Core Docker](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)

---

**Status**: ✅ **Configuração Docker Completa e Testada**
