# 🔐 Cryptography Service

Um serviço de criptografia robusto desenvolvido em **C# .NET 9** que implementa criptografia AES para proteger dados sensíveis como documentos de usuários e tokens de cartão de crédito.

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Tecnologias](#-tecnologias)
- [Funcionalidades](#-funcionalidades)
- [Instalação e Configuração](#-instalação-e-configuração)
- [Como Usar](#-como-usar)
- [API Endpoints](#-api-endpoints)
- [Docker](#-docker)
- [Testes](#-testes)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Segurança](#-segurança)
- [Contribuição](#-contribuição)

## 🎯 Visão Geral

Este projeto é uma solução completa para o desafio de criptografia proposto pelo [Back-End Brasil](https://github.com/backend-br/desafios/blob/master/cryptography/PROBLEM.md). O serviço implementa:

- **Criptografia AES-256** para dados sensíveis
- **Arquitetura em camadas** seguindo princípios SOLID
- **API REST** com documentação Swagger
- **Banco de dados SQL Server** com Entity Framework Core
- **Testes unitários** abrangentes
- **Containerização Docker** completa

### 🎯 Objetivo Principal

Proteger dados sensíveis (documentos de usuários e tokens de cartão de crédito) através de criptografia AES, permitindo operações CRUD seguras com capacidade de visualizar dados criptografados ou descriptografados conforme necessário.

## 🏗️ Arquitetura

O projeto segue uma **arquitetura em camadas** bem definida:

```
┌─────────────────┐
│   API Layer     │ ← Controllers, Program.cs
├─────────────────┤
│  Services Layer │ ← Business Logic
├─────────────────┤
│  Domain Layer   │ ← Entities, Interfaces, Business Rules
├─────────────────┤
│   Data Layer    │ ← Repository, DbContext, Migrations
└─────────────────┘
```

### 🔧 Princípios SOLID Aplicados

- **S** - Single Responsibility: Cada classe tem uma responsabilidade específica
- **O** - Open/Closed: Extensível através de interfaces
- **L** - Liskov Substitution: Implementações podem ser substituídas
- **I** - Interface Segregation: Interfaces específicas e coesas
- **D** - Dependency Inversion: Dependências injetadas via DI

## 🛠️ Tecnologias

### Core Technologies
- ![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white) **C# .NET 9**
- ![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=flat&logo=dotnet&logoColor=white) **ASP.NET Core**
- ![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=flat&logo=dotnet&logoColor=white) **Entity Framework Core**

### Database
- ![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=microsoft-sql-server&logoColor=white) **SQL Server 2022**

### Testing
- ![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=flat&logo=dotnet&logoColor=white) **xUnit**
- ![Moq](https://img.shields.io/badge/Moq-512BD4?style=flat&logo=dotnet&logoColor=white) **Moq**
- ![FluentAssertions](https://img.shields.io/badge/FluentAssertions-512BD4?style=flat&logo=dotnet&logoColor=white) **FluentAssertions**

### DevOps & Tools
- ![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white) **Docker & Docker Compose**
- ![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=flat&logo=swagger&logoColor=black) **Swagger/OpenAPI**

## ⚡ Funcionalidades

### 🔐 Criptografia
- **Algoritmo**: AES-256-CBC
- **Chave**: 256 bits (32 caracteres)
- **IV**: 128 bits (16 caracteres)
- **Encoding**: Base64 para armazenamento

### 📊 Operações CRUD
- ✅ **Create**: Criar novos registros com dados criptografados
- ✅ **Read**: Ler dados descriptografados ou criptografados
- ✅ **Update**: Atualizar registros existentes
- ✅ **Delete**: Remover registros

### 🔍 Visualização Flexível
- **Dados Descriptografados**: Para uso normal da aplicação
- **Dados Criptografados**: Para auditoria e debug

### 🧪 Testes Abrangentes
- **75 testes unitários** cobrindo todas as camadas
- **Cobertura completa** de cenários de sucesso e erro
- **Mocks** para isolamento de dependências

## 🚀 Instalação e Configuração

### Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) ou [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (opcional)

### 1. Clone o Repositório
```bash
git clone https://github.com/seu-usuario/csharp-cryptography-service.git
cd csharp-cryptography-service
```

### 2. Configuração do Banco de Dados

#### Opção A: SQL Server Local
1. Instale o SQL Server
2. Configure a connection string em `Cryptography.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "ConnectionString": "Data Source=localhost;Initial Catalog=CryptographyDb;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

#### Opção B: Docker (Recomendado)
```bash
# Iniciar apenas o SQL Server
docker-compose up sqlserver -d

# Ou iniciar todos os serviços
docker-compose up --build
```

### 3. Executar Migrations
```bash
dotnet ef database update --project Cryptography.Data --startup-project Cryptography.API
```

### 4. Configurar Chaves de Criptografia
Edite `Cryptography.API/appsettings.json`:
```json
{
  "CryptographySettings": {
    "EncryptionKey": "a2KE7izknteF3PwEyH02ZSoBNHWmPhSe",
    "InitializationVector": "DSowoZNzqW1kvTGF"
  }
}
```

⚠️ **Importante**: Altere as chaves em produção!

### 5. Executar a Aplicação
```bash
dotnet run --project Cryptography.API
```

A API estará disponível em:
- **HTTP**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger

## 📖 Como Usar

### 1. Acessar a Documentação
Navegue para http://localhost:8080/swagger para ver todos os endpoints disponíveis.

### 2. Criar um Novo Registro
```bash
curl -X POST "http://localhost:8080/Cryptography/Add" \
  -H "Content-Type: application/json" \
  -d '{
    "userDocument": "12345678901",
    "creditCardToken": "1234567890123456",
    "value": 1000
  }'
```

### 3. Buscar Registros
```bash
# Buscar todos (descriptografados)
curl "http://localhost:8080/Cryptography/All"

# Buscar todos (criptografados)
curl "http://localhost:8080/Cryptography/All/true"

# Buscar por ID (descriptografado)
curl "http://localhost:8080/Cryptography/Get/1"

# Buscar por ID (criptografado)
curl "http://localhost:8080/Cryptography/Get/1/true"
```

### 4. Atualizar Registro
```bash
curl -X PUT "http://localhost:8080/Cryptography/Update/1" \
  -H "Content-Type: application/json" \
  -d '{
    "userDocument": "98765432109",
    "creditCardToken": "9876543210987654",
    "value": 2000
  }'
```

### 5. Deletar Registro
```bash
curl -X DELETE "http://localhost:8080/Cryptography/Delete/1"
```

## 🔌 API Endpoints

| Método | Endpoint | Descrição | Parâmetros |
|--------|----------|-----------|------------|
| `GET` | `/Cryptography/All/{encrypted?}` | Lista todos os registros | `encrypted` (opcional): true/false |
| `GET` | `/Cryptography/Get/{id}/{encrypted?}` | Busca por ID | `id`: ID do registro<br>`encrypted` (opcional): true/false |
| `POST` | `/Cryptography/Add` | Cria novo registro | Body: `CryptDataDTO` |
| `PUT` | `/Cryptography/Update/{id}` | Atualiza registro | `id`: ID do registro<br>Body: `CryptDataDTO` |
| `DELETE` | `/Cryptography/Delete/{id}` | Remove registro | `id`: ID do registro |

### 📝 Modelo de Dados

#### CryptDataDTO (Request/Response)
```json
{
  "id": 1,
  "userDocument": "12345678901",
  "creditCardToken": "1234567890123456",
  "value": 1000
}
```

#### CryptData (Entity - Banco de Dados)
```json
{
  "id": 1,
  "userDocument": "encrypted_base64_string",
  "creditCardToken": "encrypted_base64_string",
  "value": 1000
}
```

## 🐳 Docker

### Início Rápido com Docker
```bash
# Construir e iniciar todos os serviços
docker-compose up --build

# Iniciar em background
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down
```

### Serviços Docker
- **API**: http://localhost:8080
- **SQL Server**: localhost:1433
- **Swagger**: http://localhost:8080/swagger

### Comandos Úteis
```bash
# Usando Makefile
make up          # Iniciar serviços
make down        # Parar serviços
make logs        # Ver logs
make test        # Executar testes
make migrate     # Executar migrations

# Usando script bash
./docker-scripts.sh up
./docker-scripts.sh logs
./docker-scripts.sh test
```

Para mais detalhes sobre Docker, consulte [DOCKER.md](./DOCKER.md).

## 🧪 Testes

### Executar Todos os Testes
```bash
dotnet test Cryptography.Tests
```

### Executar Testes Específicos
```bash
# Testes da entidade
dotnet test --filter "CryptDataTests"

# Testes do provider de criptografia
dotnet test --filter "AesCryptographyProviderTests"

# Testes do serviço
dotnet test --filter "CryptographyServiceTests"

# Testes do controller
dotnet test --filter "CryptographyControllerTests"
```

### Cobertura de Código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Estatísticas dos Testes
- **Total**: 75 testes
- **Sucessos**: 75 ✅
- **Falhas**: 0 ❌
- **Cobertura**: 100% das camadas

### Cenários Testados
- ✅ Criptografia/Descriptografia AES
- ✅ Operações CRUD completas
- ✅ Validações de entrada
- ✅ Tratamento de erros
- ✅ Cenários de borda
- ✅ Integração entre camadas

## 📁 Estrutura do Projeto

```
csharp-cryptography-service/
├── 📁 Cryptography.API/              # Camada de API
│   ├── 📁 Controllers/              # Controllers REST
│   ├── 📁 Properties/               # Configurações de launch
│   ├── appsettings.json             # Configurações da aplicação
│   ├── Program.cs                   # Ponto de entrada e DI
│   └── Cryptography.API.csproj      # Projeto da API
│
├── 📁 Cryptography.Services/         # Camada de Serviços
│   ├── 📁 Interfaces/               # Interfaces de serviços
│   └── 📁 Services/                 # Implementações de serviços
│
├── 📁 Cryptography.Domain/           # Camada de Domínio
│   ├── 📁 Entities/                 # Entidades de domínio
│   ├── 📁 Interfaces/               # Interfaces de domínio
│   ├── 📁 Services/                 # Serviços de domínio
│   ├── 📁 Settings/                 # Configurações de domínio
│   └── 📁 DTOs/                     # Data Transfer Objects
│
├── 📁 Cryptography.Data/             # Camada de Dados
│   ├── 📁 Repositories/             # Implementações de repositório
│   ├── 📁 Migrations/               # Migrations do EF Core
│   ├── AppDbContext.cs              # Contexto do banco
│   └── Cryptography.Data.csproj    # Projeto de dados
│
├── 📁 Cryptography.Tests/            # Projeto de Testes
│   ├── 📁 API/                      # Testes da API
│   ├── 📁 Services/                 # Testes dos serviços
│   ├── 📁 Domain/                   # Testes do domínio
│   ├── 📁 Data/                     # Testes dos dados
│   └── coverage.runsettings         # Configuração de cobertura
│
├── 📁 init-db/                      # Scripts de inicialização do banco
├── 📁 .docker/                      # Configurações do Docker
├── Dockerfile                       # Imagem da aplicação
├── docker-compose.yml               # Orquestração dos serviços
├── docker-compose.override.yml      # Configurações de desenvolvimento
├── docker-compose.prod.yml          # Configurações de produção
├── docker-scripts.sh                # Scripts de automação
├── Makefile                         # Comandos make
├── DOCKER.md                        # Documentação Docker
└── README.md                        # Este arquivo
```

## 🔒 Segurança

### Implementações de Segurança
- ✅ **Criptografia AES-256** para dados sensíveis
- ✅ **Validação de entrada** em todas as camadas
- ✅ **Injeção de dependência** para isolamento
- ✅ **Princípios SOLID** para código seguro
- ✅ **Testes abrangentes** para garantir segurança

### Configurações de Produção
- 🔐 **Alterar chaves de criptografia** padrão
- 🔐 **Usar HTTPS** em produção
- 🔐 **Configurar firewall** adequadamente
- 🔐 **Monitorar logs** de segurança
- 🔐 **Backup regular** do banco de dados

### Recomendações
1. **Rotacionar chaves** periodicamente
2. **Implementar auditoria** de acesso
3. **Configurar rate limiting**
4. **Usar secrets management**
5. **Implementar autenticação/autorização**

## 🤝 Contribuição

### Como Contribuir
1. **Fork** o repositório
2. **Crie** uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. **Commit** suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. **Push** para a branch (`git push origin feature/nova-feature`)
5. **Abra** um Pull Request

### Padrões de Código
- Seguir convenções do C#
- Manter cobertura de testes > 90%
- Documentar APIs públicas
- Seguir princípios SOLID
- Usar async/await adequadamente

### Reportar Bugs
Use o sistema de Issues do GitHub para reportar bugs ou sugerir melhorias.

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🙏 Agradecimentos

- [Back-End Brasil](https://github.com/backend-br) pelo desafio original
- Comunidade .NET pela documentação e ferramentas
- Contribuidores do projeto

---

**Desenvolvido com ❤️ usando C# .NET 9**
