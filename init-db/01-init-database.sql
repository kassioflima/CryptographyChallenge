-- Script de inicialização do banco de dados
-- Este script é executado automaticamente quando o container SQL Server é criado

-- Aguardar o SQL Server estar pronto
WAITFOR DELAY '00:00:10';

-- Criar o banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CryptographyDb')
BEGIN
    CREATE DATABASE CryptographyDb;
    PRINT 'Database CryptographyDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database CryptographyDb already exists.';
END

-- Usar o banco de dados
USE CryptographyDb;

-- Criar a tabela CryptData se não existir
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CryptData' AND xtype='U')
BEGIN
    CREATE TABLE CryptData (
        Id BIGINT IDENTITY(1,1) PRIMARY KEY,
        UserDocument NVARCHAR(500) NOT NULL,
        CreditCardToken NVARCHAR(500) NOT NULL,
        Value BIGINT NOT NULL
    );
    PRINT 'Table CryptData created successfully.';
END
ELSE
BEGIN
    PRINT 'Table CryptData already exists.';
END

-- Inserir dados de exemplo (opcional)
IF NOT EXISTS (SELECT * FROM CryptData)
BEGIN
    INSERT INTO CryptData (UserDocument, CreditCardToken, Value) VALUES
    ('encrypted_doc_1', 'encrypted_card_1', 1000),
    ('encrypted_doc_2', 'encrypted_card_2', 2000),
    ('encrypted_doc_3', 'encrypted_card_3', 3000);
    PRINT 'Sample data inserted successfully.';
END

PRINT 'Database initialization completed successfully.';
