-- Script para deletar todas as tabelas do projeto AutoTTU
-- Execute este script no Oracle Database antes de rodar dotnet ef database update

-- Deletar tabelas na ordem correta 
DROP TABLE "Checkin" CASCADE CONSTRAINTS;
DROP TABLE "Telemetria" CASCADE CONSTRAINTS;
DROP TABLE "Slot" CASCADE CONSTRAINTS;
DROP TABLE "Motos" CASCADE CONSTRAINTS;
DROP TABLE "Usuario" CASCADE CONSTRAINTS;
DROP TABLE "__EFMigrationsHistory" CASCADE CONSTRAINTS;

-- Verificar se as tabelas foram deletadas
SELECT TABLE_NAME FROM USER_TABLES WHERE TABLE_NAME IN ('CHECKIN', 'TELEMETRIA', 'SLOT', 'MOTOS', 'USUARIO', '__EFMIGRATIONSHISTORY');
