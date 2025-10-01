#!/usr/bin/env python3
"""
Simulador de Telemetria para AutoTTU
Envia dados de localização e status das motos para a API
"""

import requests
import json
import time
import random
import math
from datetime import datetime

# Configurações
API_BASE_URL = "http://localhost:5143/api"  # Ajuste conforme necessário
DEVICE_IDS = [1, 2, 3]  # IDs das motos cadastradas
INTERVALO_ENVIO = 30  # segundos entre envios

# Coordenadas base (São Paulo - Centro)
LATITUDE_BASE = -23.5505
LONGITUDE_BASE = -46.6333

# Raio de movimento em graus (aproximadamente 1km)
RAIO_MOVIMENTO = 0.01

def gerar_coordenadas_aleatorias():
    """Gera coordenadas aleatórias próximas à base"""
    # Gera um ângulo aleatório
    angulo = random.uniform(0, 2 * math.pi)
    
    # Gera uma distância aleatória dentro do raio
    distancia = random.uniform(0, RAIO_MOVIMENTO)
    
    # Calcula as novas coordenadas
    nova_latitude = LATITUDE_BASE + (distancia * math.cos(angulo))
    nova_longitude = LONGITUDE_BASE + (distancia * math.sin(angulo))
    
    return nova_latitude, nova_longitude

def gerar_dados_telemetria(device_id):
    """Gera dados de telemetria simulados para uma moto"""
    latitude, longitude = gerar_coordenadas_aleatorias()
    
    # Status possíveis
    status_options = ["online", "offline", "manutencao", "alugada"]
    status = random.choices(status_options, weights=[70, 5, 10, 15])[0]
    
    # Dados de telemetria
    dados = {
        "deviceId": device_id,
        "latitude": round(latitude, 8),
        "longitude": round(longitude, 8),
        "status": status,
        "temperatura": round(random.uniform(20, 45), 2) if status == "online" else None,
        "velocidade": round(random.uniform(0, 80), 2) if status == "online" else 0,
        "quilometragem": round(random.uniform(1000, 50000), 2) if status == "online" else None,
        "nivelCombustivel": round(random.uniform(10, 100), 2) if status == "online" else None,
        "rotacaoMotor": round(random.uniform(1000, 8000), 2) if status == "online" else None,
        "observacoes": f"Simulação automática - {datetime.now().strftime('%H:%M:%S')}"
    }
    
    return dados

def enviar_telemetria(dados):
    """Envia dados de telemetria para a API"""
    try:
        url = f"{API_BASE_URL}/telemetria"
        headers = {
            "Content-Type": "application/json"
        }
        
        response = requests.post(url, json=dados, headers=headers, verify=False)
        
        if response.status_code in [200, 201]:
            print(f"✅ Dados enviados com sucesso para moto {dados['deviceId']}: {dados['status']}")
        else:
            print(f"❌ Erro ao enviar dados para moto {dados['deviceId']}: {response.status_code} - {response.text}")
            
    except requests.exceptions.RequestException as e:
        print(f"❌ Erro de conexão: {e}")

def main():
    """Função principal do simulador"""
    print("🚀 Iniciando simulador de telemetria AutoTTU")
    print(f"📡 Enviando dados para: {API_BASE_URL}")
    print(f"🛵 Dispositivos: {DEVICE_IDS}")
    print(f"⏱️  Intervalo: {INTERVALO_ENVIO} segundos")
    print("=" * 50)
    
    contador = 0
    
    try:
        while True:
            contador += 1
            print(f"\n📊 Ciclo {contador} - {datetime.now().strftime('%H:%M:%S')}")
            
            # Envia dados para cada dispositivo
            for device_id in DEVICE_IDS:
                dados = gerar_dados_telemetria(device_id)
                enviar_telemetria(dados)
                time.sleep(1)  # Pequena pausa entre envios
            
            print(f"⏳ Aguardando {INTERVALO_ENVIO} segundos...")
            time.sleep(INTERVALO_ENVIO)
            
    except KeyboardInterrupt:
        print("\n\n🛑 Simulador interrompido pelo usuário")
        print("👋 Até logo!")

if __name__ == "__main__":
    main()
