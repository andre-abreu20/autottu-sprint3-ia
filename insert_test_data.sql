-- Script para inserir dados de teste no banco Oracle
-- Execute este script no Oracle SQL Developer

-- Inserir motos de teste
INSERT INTO "Motos" ("Modelo", "Marca", "Ano", "Placa", "AtivoChar", "FotoUrl", "Latitude", "Longitude", "Endereco", "UltimaAtualizacao") 
VALUES ('CB 600F', 'Honda', 2020, 'ABC-1234', 'S', 'https://example.com/moto1.jpg', -23.5505, -46.6333, 'São Paulo, SP', SYSTIMESTAMP);

INSERT INTO "Motos" ("Modelo", "Marca", "Ano", "Placa", "AtivoChar", "FotoUrl", "Latitude", "Longitude", "Endereco", "UltimaAtualizacao") 
VALUES ('MT-07', 'Yamaha', 2021, 'DEF-5678', 'S', 'https://example.com/moto2.jpg', -23.5505, -46.6333, 'São Paulo, SP', SYSTIMESTAMP);

INSERT INTO "Motos" ("Modelo", "Marca", "Ano", "Placa", "AtivoChar", "FotoUrl", "Latitude", "Longitude", "Endereco", "UltimaAtualizacao") 
VALUES ('Z650', 'Kawasaki', 2022, 'GHI-9012', 'S', 'https://example.com/moto3.jpg', -23.5505, -46.6333, 'São Paulo, SP', SYSTIMESTAMP);

-- Inserir usuário de teste
INSERT INTO "Usuario" ("Nome", "Email", "Senha", "Telefone") 
VALUES ('João Silva', 'joao@email.com', '123456', '11999999999');

-- Inserir slots de teste
INSERT INTO "Slot" ("IdMoto", "AtivoChar") VALUES (1, 'S');
INSERT INTO "Slot" ("IdMoto", "AtivoChar") VALUES (2, 'S');
INSERT INTO "Slot" ("IdMoto", "AtivoChar") VALUES (3, 'S');

COMMIT;
