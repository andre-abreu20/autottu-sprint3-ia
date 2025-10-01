#!/usr/bin/env node
/**
 * Simulador de Telemetria para AutoTTU
 * Envia dados de localização e status das motos para a API
 */

const axios = require("axios");

// Configurações
const API_BASE_URL = "http://localhost:5143/api"; // Ajuste conforme necessário
const DEVICE_IDS = [1, 2, 3]; // IDs das motos cadastradas
const INTERVALO_ENVIO = 30000; // milissegundos entre envios

// Coordenadas base (São Paulo - Centro)
const LATITUDE_BASE = -23.5505;
const LONGITUDE_BASE = -46.6333;

// Raio de movimento em graus (aproximadamente 1km)
const RAIO_MOVIMENTO = 0.01;

/**
 * Gera coordenadas aleatórias próximas à base
 */
function gerarCoordenadasAleatorias() {
  // Gera um ângulo aleatório
  const angulo = Math.random() * 2 * Math.PI;

  // Gera uma distância aleatória dentro do raio
  const distancia = Math.random() * RAIO_MOVIMENTO;

  // Calcula as novas coordenadas
  const novaLatitude = LATITUDE_BASE + distancia * Math.cos(angulo);
  const novaLongitude = LONGITUDE_BASE + distancia * Math.sin(angulo);

  return {
    latitude: parseFloat(novaLatitude.toFixed(8)),
    longitude: parseFloat(novaLongitude.toFixed(8)),
  };
}

/**
 * Gera dados de telemetria simulados para uma moto
 */
function gerarDadosTelemetria(deviceId) {
  const { latitude, longitude } = gerarCoordenadasAleatorias();

  // Status possíveis com pesos
  const statusOptions = [
    { status: "online", peso: 70 },
    { status: "offline", peso: 5 },
    { status: "manutencao", peso: 10 },
    { status: "alugada", peso: 15 },
  ];

  const status =
    statusOptions[Math.floor(Math.random() * statusOptions.length)].status;

  // Dados de telemetria
  const dados = {
    deviceId: deviceId,
    latitude: latitude,
    longitude: longitude,
    status: status,
    temperatura:
      status === "online"
        ? parseFloat((Math.random() * 25 + 20).toFixed(2))
        : null,
    velocidade:
      status === "online" ? parseFloat((Math.random() * 80).toFixed(2)) : 0,
    bateria:
      status === "online"
        ? parseFloat((Math.random() * 80 + 20).toFixed(2))
        : null,
    observacoes: `Simulação automática - ${new Date().toLocaleTimeString()}`,
  };

  return dados;
}

/**
 * Envia dados de telemetria para a API
 */
async function enviarTelemetria(dados) {
  try {
    const url = `${API_BASE_URL}/telemetria`;
    const headers = {
      "Content-Type": "application/json",
    };

    const response = await axios.post(url, dados, {
      headers,
      timeout: 5000,
    });

    if (response.status === 201) {
      console.log(
        `✅ Dados enviados com sucesso para moto ${dados.deviceId}: ${dados.status}`
      );
    } else {
      console.log(
        `❌ Erro ao enviar dados para moto ${dados.deviceId}: ${response.status}`
      );
    }
  } catch (error) {
    if (error.response) {
      console.log(
        `❌ Erro HTTP para moto ${dados.deviceId}: ${error.response.status} - ${error.response.data}`
      );
    } else if (error.request) {
      console.log(
        `❌ Erro de conexão para moto ${dados.deviceId}: ${error.message}`
      );
    } else {
      console.log(`❌ Erro: ${error.message}`);
    }
  }
}

/**
 * Função principal do simulador
 */
async function main() {
  console.log("🚀 Iniciando simulador de telemetria AutoTTU");
  console.log(`📡 Enviando dados para: ${API_BASE_URL}`);
  console.log(`🛵 Dispositivos: ${DEVICE_IDS.join(", ")}`);
  console.log(`⏱️  Intervalo: ${INTERVALO_ENVIO / 1000} segundos`);
  console.log("=".repeat(50));

  let contador = 0;

  try {
    while (true) {
      contador++;
      console.log(
        `\n📊 Ciclo ${contador} - ${new Date().toLocaleTimeString()}`
      );

      // Envia dados para cada dispositivo
      for (const deviceId of DEVICE_IDS) {
        const dados = gerarDadosTelemetria(deviceId);
        await enviarTelemetria(dados);
        await new Promise((resolve) => setTimeout(resolve, 1000)); // Pausa de 1s entre envios
      }

      console.log(`⏳ Aguardando ${INTERVALO_ENVIO / 1000} segundos...`);
      await new Promise((resolve) => setTimeout(resolve, INTERVALO_ENVIO));
    }
  } catch (error) {
    console.error("\n❌ Erro no simulador:", error.message);
    process.exit(1);
  }
}

// Tratamento de interrupção
process.on("SIGINT", () => {
  console.log("\n\n🛑 Simulador interrompido pelo usuário");
  console.log("👋 Até logo!");
  process.exit(0);
});

// Verificar se axios está instalado
try {
  require.resolve("axios");
  main();
} catch (error) {
  console.log("❌ Axios não encontrado. Instale com: npm install axios");
  console.log("💡 Ou use o simulador Python: python simulador_telemetria.py");
  process.exit(1);
}
