#!/bin/bash

# Script para gerenciar o ambiente Docker do projeto Cryptography
# Uso: ./docker-scripts.sh [comando]

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para mostrar ajuda
show_help() {
    echo -e "${BLUE}Script de gerenciamento Docker para Cryptography Service${NC}"
    echo ""
    echo "Comandos disponíveis:"
    echo "  build     - Constrói as imagens Docker"
    echo "  up        - Inicia os serviços"
    echo "  down      - Para e remove os serviços"
    echo "  restart   - Reinicia os serviços"
    echo "  logs      - Mostra os logs dos serviços"
    echo "  clean     - Remove volumes e imagens"
    echo "  migrate   - Executa migrations do banco"
    echo "  test      - Executa testes"
    echo "  shell     - Abre shell no container da API"
    echo "  db-shell  - Abre shell SQL no banco"
    echo ""
}

# Função para construir as imagens
build_images() {
    echo -e "${YELLOW}Construindo imagens Docker...${NC}"
    docker-compose build --no-cache
    echo -e "${GREEN}Imagens construídas com sucesso!${NC}"
}

# Função para iniciar os serviços
start_services() {
    echo -e "${YELLOW}Iniciando serviços...${NC}"
    docker-compose up -d
    echo -e "${GREEN}Serviços iniciados!${NC}"
    echo -e "${BLUE}API disponível em: http://localhost:8080${NC}"
    echo -e "${BLUE}Swagger disponível em: http://localhost:8080/swagger${NC}"
    echo -e "${BLUE}SQL Server disponível em: localhost:1433${NC}"
}

# Função para parar os serviços
stop_services() {
    echo -e "${YELLOW}Parando serviços...${NC}"
    docker-compose down
    echo -e "${GREEN}Serviços parados!${NC}"
}

# Função para reiniciar os serviços
restart_services() {
    echo -e "${YELLOW}Reiniciando serviços...${NC}"
    docker-compose restart
    echo -e "${GREEN}Serviços reiniciados!${NC}"
}

# Função para mostrar logs
show_logs() {
    echo -e "${YELLOW}Mostrando logs dos serviços...${NC}"
    docker-compose logs -f
}

# Função para limpar volumes e imagens
clean_docker() {
    echo -e "${YELLOW}Limpando volumes e imagens...${NC}"
    docker-compose down -v --rmi all
    docker system prune -f
    echo -e "${GREEN}Limpeza concluída!${NC}"
}

# Função para executar migrations
run_migrations() {
    echo -e "${YELLOW}Executando migrations...${NC}"
    docker-compose exec cryptography-api dotnet ef database update --project Cryptography.Data --startup-project Cryptography.API
    echo -e "${GREEN}Migrations executadas!${NC}"
}

# Função para executar testes
run_tests() {
    echo -e "${YELLOW}Executando testes...${NC}"
    docker-compose exec cryptography-api dotnet test Cryptography.Tests
    echo -e "${GREEN}Testes executados!${NC}"
}

# Função para abrir shell na API
open_api_shell() {
    echo -e "${YELLOW}Abrindo shell na API...${NC}"
    docker-compose exec cryptography-api /bin/bash
}

# Função para abrir shell SQL
open_db_shell() {
    echo -e "${YELLOW}Abrindo shell SQL...${NC}"
    docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Cryptography@123
}

# Main script
case "${1:-help}" in
    "build")
        build_images
        ;;
    "up")
        start_services
        ;;
    "down")
        stop_services
        ;;
    "restart")
        restart_services
        ;;
    "logs")
        show_logs
        ;;
    "clean")
        clean_docker
        ;;
    "migrate")
        run_migrations
        ;;
    "test")
        run_tests
        ;;
    "shell")
        open_api_shell
        ;;
    "db-shell")
        open_db_shell
        ;;
    "help"|*)
        show_help
        ;;
esac
