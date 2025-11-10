# 🏍️ AutoTTU - Sistema de Monitoramento de Motos em Tempo Real

Sistema completo de monitoramento de motos em tempo real, desenvolvido com **ASP.NET Core** e **Entity Framework Core**, incluindo dashboard web interativo e simuladores de telemetria.

---

## 🚀 Tecnologias

- **.NET 8** / **ASP.NET Core**
- **C# 12**
- **Entity Framework Core** com **Oracle Database**
- **Swagger** / **OpenAPI** para documentação de endpoints
- **HTML5**, **CSS3**, **JavaScript** (Dashboard)
- **Leaflet.js** (Mapas interativos)
- **Chart.js** (Gráficos)
- **Python** / **Node.js** (Simuladores)

---

## 📋 Funcionalidades

### 🔧 API .NET Core

- ✅ **CRUD completo** de Slots, Usuários, Motos e Checkins
- ✅ **Sistema de Telemetria** com localização GPS
- ✅ **Validação de dados** e tratamento de erros
- ✅ **Documentação automática** via Swagger UI
- ✅ **CORS configurado** para acesso do dashboard
- ✅ **Migrations** do Entity Framework
- ✅ **Status codes corretos** (200, 201, 400, 404, 500)

### 📊 Dashboard Web

- ✅ **Mapa interativo** com localização das motos
- ✅ **Indicadores de status** em tempo real
- ✅ **Gráficos** de status e temperatura
- ✅ **Lista detalhada** de todas as motos
- ✅ **Atualização automática** a cada 30 segundos
- ✅ **Design responsivo** e moderno

### 🤖 Simuladores de Telemetria

- ✅ **Script Python** para envio de dados
- ✅ **Script Node.js** alternativo
- ✅ **Geração de coordenadas** aleatórias
- ✅ **Simulação de diferentes status** das motos
- ✅ **Dados realistas** para motos (quilometragem, combustível, RPM)

---

## 🛠️ Instalação e Configuração

### 1. Configurar a API .NET

```bash
# Navegar para o diretório da API
cd Autottu-API/AutoTTU

# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update

# Executar a API
dotnet run
```

A API estará disponível em `http://localhost:5143`

### 2. Inserir Dados de Teste

Após executar a API, insira dados de teste para testar o sistema:

#### Opção A: Via API (Recomendado)

```bash
# Com a API rodando, execute:
Invoke-RestMethod -Uri "http://localhost:5143/api/testdata/create-test-data" -Method POST
```

#### Opção B: Via SQL (Alternativo)

Execute o script `insert_test_data.sql` no Oracle Database:

```sql
-- Inserir motos de teste
INSERT INTO "Motos" ("Modelo", "Marca", "Ano", "Placa", "AtivoChar", "FotoUrl", "Latitude", "Longitude", "Endereco", "UltimaAtualizacao")
VALUES ('CB 600F', 'Honda', 2020, 'ABC-1234', 'S', 'https://example.com/moto1.jpg', -23.5505, -46.6333, 'São Paulo, SP', SYSTIMESTAMP);

-- ... (veja o arquivo insert_test_data.sql para o script completo)
```

### 3. Configurar o Banco de Dados

Certifique-se de que o `appsettings.json` está configurado com a string de conexão Oracle:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost:1521/XE;User Id=seu_usuario;Password=sua_senha;"
  }
}
```

### 4. Executar Simuladores de Telemetria

#### Opção A: Python

```bash
# Instalar dependências
pip install requests

# Executar simulador
python simulador_telemetria.py
```

#### Opção B: Node.js

```bash
# Instalar dependências
npm install axios

# Executar simulador
node simulador_telemetria.js
```

### 5. Abrir Dashboard

Abra o arquivo `dashboard.html` em um navegador web ou sirva-o através de um servidor HTTP local.

---

## 📊 Endpoints da API

### Motos

- **GET** `/api/motos` - Listar todas as motos
- **GET** `/api/motos/{id}` - Buscar moto por ID
- **POST** `/api/motos` - Criar nova moto
- **PUT** `/api/motos/{id}` - Atualizar moto
- **DELETE** `/api/motos/{id}` - Deletar moto

### Telemetria

- **POST** `/api/telemetria` - Enviar dados de telemetria
- **GET** `/api/telemetria/ultimas?limit=50` - Buscar últimas telemetrias
- **GET** `/api/telemetria/moto/{deviceId}` - Telemetrias de uma moto específica
- **GET** `/api/telemetria/motos-localizacao` - Motos com localização atual

### Slots

- **GET** `/api/slots` - Listar todos os slots
- **GET** `/api/slots/{id}` - Buscar slot por ID
- **POST** `/api/slots` - Criar novo slot
- **PUT** `/api/slots/{id}` - Atualizar slot
- **DELETE** `/api/slots/{id}` - Deletar slot

### Usuários

- **GET** `/api/usuarios` - Listar todos os usuários
- **GET** `/api/usuarios/{id}` - Buscar usuário por ID
- **POST** `/api/usuarios` - Criar novo usuário
- **PUT** `/api/usuarios/{id}` - Atualizar usuário
- **DELETE** `/api/usuarios/{id}` - Deletar usuário

### Checkins

- **GET** `/api/checkins` - Listar todos os checkins
- **GET** `/api/checkins/{id}` - Buscar checkin por ID
- **POST** `/api/checkins` - Criar novo checkin
- **PUT** `/api/checkins/{id}` - Atualizar checkin
- **DELETE** `/api/checkins/{id}` - Deletar checkin

### Dados de Teste

- **POST** `/api/testdata/create-test-data` - Criar dados de teste

---

## 📡 Dados de Telemetria

### Exemplo de Dados Enviados

```json
{
  "deviceId": 1,
  "latitude": -23.5505,
  "longitude": -46.6333,
  "status": "online",
  "temperatura": 25.5,
  "velocidade": 45.2,
  "quilometragem": 15000.0,
  "nivelCombustivel": 75.0,
  "rotacaoMotor": 3500.0,
  "observacoes": "Simulação automática"
}
```

### Campos de Telemetria

- **deviceId** - ID da moto
- **latitude/longitude** - Coordenadas GPS
- **status** - Status da moto (online, offline, manutencao, alugada)
- **temperatura** - Temperatura do motor (°C)
- **velocidade** - Velocidade atual (km/h)
- **quilometragem** - Quilômetros rodados
- **nivelCombustivel** - Nível de combustível (%)
- **rotacaoMotor** - Rotação do motor (RPM)
- **observacoes** - Observações adicionais

---

## 🎯 Status das Motos

- **🟢 Online** - Moto operacional e conectada
- **🔴 Offline** - Moto desconectada ou sem sinal
- **🟠 Manutenção** - Moto em manutenção
- **🔵 Alugada** - Moto alugada por cliente

---

## 🗺️ Funcionalidades do Dashboard

### Mapa Interativo

- Visualização em tempo real das motos
- Marcadores coloridos por status
- Popup com informações detalhadas
- Zoom automático para mostrar todas as motos

### Estatísticas

- Total de motos cadastradas
- Contagem por status
- Gráfico de pizza com distribuição
- Gráfico de linha com temperatura média

### Lista de Motos

- Cards individuais para cada moto
- Informações detalhadas (placa, ano, status)
- Dados de telemetria (temperatura, velocidade, quilometragem, combustível, RPM)
- Timestamp da última atualização

---

## ⚙️ Configurações

### Simulador Python

Edite as configurações no arquivo `simulador_telemetria.py`:

```python
API_BASE_URL = "http://localhost:5143/api"  # URL da API
DEVICE_IDS = [1, 2, 3]  # IDs das motos
INTERVALO_ENVIO = 30  # segundos entre envios
```

### Simulador Node.js

Edite as configurações no arquivo `simulador_telemetria.js`:

```javascript
const API_BASE_URL = "http://localhost:5143/api";
const DEVICE_IDS = [1, 2, 3];
const INTERVALO_ENVIO = 30000; // milissegundos
```

### Dashboard

Edite a URL da API no arquivo `dashboard.html`:

```javascript
const API_BASE_URL = "http://localhost:5143/api";
```

---

# AutoTTU ESP32 – Telemetria via HiveMQ

Firmware `esp32.ino` publica a telemetria da moto no **HiveMQ** usando MQTT.  
Os dados são consumidos pela API .NET (`MqttTelemetryBackgroundService`) e gravados no Oracle.

---

## 1. Hardware / Simulação Suportados

- ESP32 DevKit (físico) ou simulação Wokwi.
- Sensores:
  - Temperatura do motor (`DS18B20`).
  - Nível de combustível (potenciômetro).
  - Indicação visual (LED de alerta).
- O tópico MQTT padrão é `autottu/motos/1`.

---

## 2. Configurar o Firmware

Arquivo principal: `esp32.ino`.

1. Atualize as credenciais MQTT e Wi-Fi:
   ```cpp
   const char* WIFI_SSID      = "SUA_REDE";
   const char* WIFI_PASSWORD  = "SUA_SENHA";
   const char* MQTT_BROKER    = "broker.hivemq.com";
   const uint16_t MQTT_PORT   = 1883;        // 8883 para TLS
   const char* MQTT_TOPIC     = "autottu/motos/1";
   const char* MQTT_CLIENT_ID = "esp32-autottu";
   ```
2. Se precisar de TLS (HiveMQ Cloud), adapte o sketch para `WiFiClientSecure`, carregue o certificado baixado por `fetch_cert.py` e use porta 8883.
3. Ajuste as diretivas dos sensores conforme o cenário:
   ```cpp
   #define USE_REAL_TEMPERATURE   0
   #define USE_REAL_FUEL_LEVEL    0
   #define USE_REAL_SPEED_SENSOR  0
   #define USE_GPS_MODULE         0
   ```
4. Compile e faça upload para o ESP32 físico via Arduino IDE ou PlatformIO.

---

## 3. Emulação com Wokwi (linha de comando)

Requisitos: Node.js 18+, `npm`, `arduino-cli`.

```bash
npm install -g @wokwi/cli
arduino-cli core install esp32:esp32

cd iot/esp32
arduino-cli compile --fqbn esp32:esp32:esp32 esp32.ino --output-dir build
wokwi start
```

- `wokwi start` lê `diagram.json` e `wokwi.toml`, abrindo o simulador no navegador.
- O serial monitor mostra logs de Wi-Fi, MQTT e JSON publicado.
- Ajuste o sketch antes de compilar para usar suas credenciais/broker.

---

## 4. Emulação Web (wokwi.com)

1. Acesse [https://wokwi.com](https://wokwi.com) e escolha o projeto ESP32.
2. Importe `diagram.json` e o arquivo `esp32.ino`.
3. Clique em **Start Simulation**.
4. Observe o console serial para conferir as publicações MQTT.

---

## 5. Estrutura de Arquivos

- `esp32.ino` – firmware principal.
- `diagram.json` – layout de sensores no Wokwi.
- `wokwi.toml` – configurações da CLI Wokwi (firmware/elf).
- `fetch_cert.py` – utilitário para baixar o certificado raiz do HiveMQ Cloud.
- `libraries.txt` – lista de bibliotecas usadas.

---

## 6. Payload Publicado

Exemplo de mensagem enviada para o HiveMQ:

```json
{
  "deviceId": 1,
  "latitude": -23.55052,
  "longitude": -46.63331,
  "status": "em_uso",
  "temperatura": 78.5,
  "velocidade": 42.3,
  "quilometragem": 15210.7,
  "nivelCombustivel": 68.4,
  "rotacaoMotor": 3100.0,
  "observacoes": "Moto em utilização normal"
}
```

Certifique-se de que o `deviceId` exista na API (`/api/motos`) para que a mensagem seja persistida.

---

**Equipe AutoTTU – telemetria ponta a ponta.**



## 🚨 Solução de Problemas

### Erro de CORS

Se houver problemas de CORS, verifique se o CORS está configurado no `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### Certificado SSL

Para desenvolvimento local, o simulador ignora certificados SSL inválidos. Em produção, configure certificados válidos.

### Banco de Dados

Certifique-se de que as migrations foram aplicadas:

```bash
dotnet ef database update
```

### Erro de Conexão

Se houver erro de conexão com a API, verifique:

1. Se a API está rodando (`dotnet run`)
2. Se a porta está correta (5143)
3. Se o protocolo está correto (HTTP, não HTTPS)

---

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

---

## 👥 Integrantes do Projeto

| Nome                         | RM     | GitHub                                             |
| ---------------------------- | ------ | -------------------------------------------------- |
| André Luís Mesquita de Abreu | 558159 | [@andre-abreu20](https://github.com/andre-abreu20) |
| Maria Eduarda Brigidio       | 558575 | [@dudabrigidio](https://github.com/dudabrigidio)   |
| Rafael Bompadre Lima         | 556459 | [@Rafa130206](https://github.com/Rafa130206)       |

---

**Desenvolvido com ❤️ para o AutoTTU**


