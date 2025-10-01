# 🏍️ Dashboard AutoTTU - Monitoramento de Motos em Tempo Real

Este projeto implementa um sistema completo de monitoramento de motos em tempo real, incluindo:

- **API .NET** com endpoints para telemetria
- **Dashboard web** com visualização em tempo real
- **Scripts simuladores** para gerar dados de teste
- **Mapa interativo** com localização das motos
- **Gráficos** de status e temperatura

## 🚀 Funcionalidades

### Backend (.NET)

- ✅ Endpoints de telemetria (`POST /api/telemetria`, `GET /api/telemetria/ultimas`)
- ✅ Modelo de dados com localização (latitude, longitude)
- ✅ Relacionamento entre motos e telemetria
- ✅ CORS configurado para acesso do dashboard
- ✅ Migrations do Entity Framework

### Dashboard Web

- ✅ Mapa interativo com localização das motos
- ✅ Indicadores de status em tempo real
- ✅ Gráficos de status e temperatura
- ✅ Lista detalhada de todas as motos
- ✅ Atualização automática a cada 30 segundos
- ✅ Design responsivo e moderno

### Simuladores

- ✅ Script Python para envio de dados
- ✅ Script Node.js alternativo
- ✅ Geração de coordenadas aleatórias
- ✅ Simulação de diferentes status das motos

## 📋 Pré-requisitos

- .NET 8.0 SDK
- Python 3.7+ (para simulador Python)
- Node.js 14+ (para simulador Node.js)
- Navegador web moderno

## 🛠️ Instalação e Configuração

### 1. Configurar a API

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

A API estará disponível em `https://localhost:7000`

### 2. Configurar o Banco de Dados

Certifique-se de que o `appsettings.json` está configurado com a string de conexão correta:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "sua_string_de_conexao_aqui"
  }
}
```

### 3. Executar Simuladores

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

### 4. Abrir Dashboard

Abra o arquivo `dashboard.html` em um navegador web ou sirva-o através de um servidor HTTP local.

## 📊 Endpoints da API

### Telemetria

- **POST** `/api/telemetria` - Enviar dados de telemetria
- **GET** `/api/telemetria/ultimas?limit=50` - Buscar últimas telemetrias
- **GET** `/api/telemetria/moto/{deviceId}` - Telemetrias de uma moto específica
- **GET** `/api/telemetria/motos-localizacao` - Motos com localização atual

### Exemplo de Dados de Telemetria

```json
{
  "deviceId": 1,
  "latitude": -23.5505,
  "longitude": -46.6333,
  "status": "online",
  "temperatura": 25.5,
  "velocidade": 45.2,
  "bateria": 85.0,
  "observacoes": "Simulação automática"
}
```

## 🎯 Status das Motos

- **🟢 Online** - Moto operacional e conectada
- **🔴 Offline** - Moto desconectada ou sem sinal
- **🟠 Manutenção** - Moto em manutenção
- **🔵 Alugada** - Moto alugada por cliente

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
- Dados de telemetria (temperatura, velocidade, bateria)
- Timestamp da última atualização

## 🔧 Configurações

### Simulador Python

Edite as configurações no arquivo `simulador_telemetria.py`:

```python
API_BASE_URL = "https://localhost:7000/api"  # URL da API
DEVICE_IDS = [1, 2, 3]  # IDs das motos
INTERVALO_ENVIO = 30  # segundos entre envios
```

### Simulador Node.js

Edite as configurações no arquivo `simulador_telemetria.js`:

```javascript
const API_BASE_URL = "https://localhost:7000/api";
const DEVICE_IDS = [1, 2, 3];
const INTERVALO_ENVIO = 30000; // milissegundos
```

### Dashboard

Edite a URL da API no arquivo `dashboard.html`:

```javascript
const API_BASE_URL = "https://localhost:7000/api";
```

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

## 📈 Próximos Passos

- [ ] Implementar autenticação e autorização
- [ ] Adicionar notificações push
- [ ] Implementar histórico de rotas
- [ ] Adicionar alertas automáticos
- [ ] Implementar relatórios em PDF
- [ ] Adicionar suporte a MQTT
- [ ] Implementar cache Redis

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

## 📞 Suporte

Para suporte ou dúvidas, entre em contato através dos issues do GitHub ou email.

---

**Desenvolvido com ❤️ para o AutoTTU**
