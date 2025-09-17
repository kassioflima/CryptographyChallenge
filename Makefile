# Makefile para Cryptography Service Docker
# Uso: make [comando]

.PHONY: help build up down restart logs clean migrate test shell db-shell

# Comandos padrão
help:
	@echo "Comandos disponíveis:"
	@echo "  build     - Constrói as imagens Docker"
	@echo "  up        - Inicia os serviços"
	@echo "  down      - Para e remove os serviços"
	@echo "  restart   - Reinicia os serviços"
	@echo "  logs      - Mostra os logs dos serviços"
	@echo "  clean     - Remove volumes e imagens"
	@echo "  migrate   - Executa migrations do banco"
	@echo "  test      - Executa testes"
	@echo "  shell     - Abre shell no container da API"
	@echo "  db-shell  - Abre shell SQL no banco"

# Construir imagens
build:
	docker-compose build --no-cache

# Iniciar serviços
up:
	docker-compose up -d
	@echo "Serviços iniciados!"
	@echo "API: http://localhost:8080"
	@echo "Swagger: http://localhost:8080/swagger"
	@echo "SQL Server: localhost:1433"

# Parar serviços
down:
	docker-compose down

# Reiniciar serviços
restart:
	docker-compose restart

# Mostrar logs
logs:
	docker-compose logs -f

# Limpar volumes e imagens
clean:
	docker-compose down -v --rmi all
	docker system prune -f

# Executar migrations
migrate:
	docker-compose exec cryptography-api dotnet ef database update --project Cryptography.Data --startup-project Cryptography.API

# Executar testes
test:
	docker-compose exec cryptography-api dotnet test Cryptography.Tests

# Shell na API
shell:
	docker-compose exec cryptography-api /bin/bash

# Shell SQL
db-shell:
	docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Cryptography@123

# Deploy em produção
deploy-prod:
	docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Backup do banco
backup:
	docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Cryptography@123 -Q "BACKUP DATABASE CryptographyDb TO DISK = '/var/opt/mssql/backup/CryptographyDb.bak'"

# Status dos serviços
status:
	docker-compose ps
