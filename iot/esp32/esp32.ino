/**
 * AutoTTU ESP32 Telemetry Publisher - VERSÃO WOKWI (MQTT genérico)
 */

#include <WiFi.h>
#include <WiFiClientSecure.h>
#define MQTT_MAX_PACKET_SIZE 512
#include <PubSubClient.h>
#define USE_HTTP_BACKEND 0

#if USE_HTTP_BACKEND
#include <HTTPClient.h>
#endif

 // === Configurações de rede ===
 const char* API_BASE_URL = "http://host.wokwi.internal:5143"; 
 const char* WIFI_SSID = "Wokwi-GUEST";
 const char* WIFI_PASSWORD = "";
  
 // === Configurações MQTT genérico ===
 const char *MQTT_BROKER    = "b285d70f23b343eea183db3bb36fb891.s1.eu.hivemq.cloud";
 const uint16_t MQTT_PORT   = 8883;
 const char *MQTT_USERNAME  = "esp32-autottu";             
const char *MQTT_PASSWORD_ = "##!U6d5tm3ZhpYB";
 const char *MQTT_TOPIC     = "autottu/motos/1";
 const char *MQTT_CLIENT_ID = "autottu-esp32-01";

// Certificado raiz da DigiCert (usado pela HiveMQ Cloud)
// Fonte: https://www.hivemq.com/docs/cloud/clusters/mqtt-clients/ssl-connections.html
static const char HIVEMQ_CLOUD_CA_CERT[] PROGMEM = R"EOF(
-----BEGIN CERTIFICATE-----
MIIDrzCCApegAwIBAgIQCDvgVpKDaszASItHsaW2XzANBgkqhkiG9w0BAQUFADBh
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMRcwFQYDVQQDEw5EaWdpQ2VydCBSb290IENBMB4XDTA2
MTExMDAwMDAwMFoXDTMxMTExMDAwMDAwMFowYTELMAkGA1UEBhMCVVMxFTATBgNV
BAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3LmRpZ2ljZXJ0LmNvbTEXMBUG
A1UEAxMORGlnaUNlcnQgUm9vdCBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCC
AQoCggEBAK0OFc7kQJ5WQ+Vt3E4JKEfi3UYxaDJzD+RQDeJ1Pzv/6z88kAOrhg95
f0FD42mCKj1mDjX6ixQwNQ8Qf95cKZ9dDin7Ezvl0KQnJqzylr9HWwxX6NGI8kaR
T1RXV4AUT7rjE4zWe733Ts7po18oY+w6mHaQJi1oTg7BEYgvEnYcU/RyXmqf1dau
+2RAv/1X7h9IraK27HJfv/op4O81w8vLqJvVEiZl+yL9MNO4ZpjQ2u+vJYJTuGHW
X0um0oC8AKjK3fFSkdH0Yy41JG+R3lHxQrnoY78h1YaeXQW8I3zZscFM82X3CYd6
vPeU5bQ3mDlZAs+UeNmk+ogHRAcCAwEAAaNjMGEwDwYDVR0TAQH/BAUwAwEB/zAO
BgNVHQ8BAf8EBAMCAQYwHQYDVR0OBBYEFJczMNo0XF1IJcYadPZbk6j0P8gPMB8G
A1UdIwQYMBaAFJczMNo0XF1IJcYadPZbk6j0P8gPMA0GCSqGSIb3DQEBBQUAA4GB
AHbhEm5OSHiO8Ez9tAMyF8asbye3wA1fwByHRl8XGYAJCCidvYxQY6vSJ6npt+b3
fqJd7jMA4brmCmsO3IE3gp0cDx07zd5UpykJkNjTV48CQhIHEvYyIF7G9wq2D6v7
Lwy1gNOfWGgkz77/MkzzkIq6VqDBbLDC/CMdCklh
-----END CERTIFICATE-----
)EOF";
 
 // === Flags para sensores reais (no Wokwi deixe tudo 0) ===
 #define USE_REAL_TEMPERATURE   0
 #define USE_REAL_FUEL_LEVEL    0
 #define USE_REAL_SPEED_SENSOR  0
 #define USE_GPS_MODULE         0
 
 // === Pinos de sensores (batem com o diagrama do Wokwi) ===
 const uint8_t PIN_TEMPERATURE  = 4;   // DS18B20
 const uint8_t PIN_FUEL_LEVEL   = 34;  // Potenciômetro
 const uint8_t PIN_SPEED_SENSOR = 27;  // Hall (não usado na simulação)
 const uint8_t PIN_LED_ALERT    = 2;   // LED vermelho
 
 // === Dependências opcionais condicionais ===
 #if USE_REAL_TEMPERATURE
   #include <OneWire.h>
   #include <DallasTemperature.h>
   OneWire oneWire(PIN_TEMPERATURE);
   DallasTemperature temperatureSensor(&oneWire);
 #endif
 
 #if USE_GPS_MODULE
   #include <TinyGPSPlus.h>
   HardwareSerial gpsSerial(2);
   TinyGPSPlus gps;
 #endif
 
WiFiClientSecure secureClient;
PubSubClient mqttClient(secureClient);
 
 // === Estrutura de telemetria ===
 struct TelemetryData {
   uint8_t deviceId;
   float latitude;
   float longitude;
   float temperaturaMotor;
   float velocidade;
   float nivelCombustivel;
   float quilometragem;
   float rotacaoMotor;
   String status;
   String observacoes;
 };
 
 // === Estado atual (simulado) ===
 TelemetryData currentState = {
   .deviceId         = 1,
   .latitude         = -23.55052f,
   .longitude        = -46.63331f,
   .temperaturaMotor = 70.0f,
   .velocidade       = 0.0f,
   .nivelCombustivel = 75.0f,
   .quilometragem    = 15200.0f,
   .rotacaoMotor     = 1200.0f,
   .status           = "offline",
   .observacoes      = "Inicializando"
 };
 
 unsigned long lastPublish = 0;
 const unsigned long PUBLISH_INTERVAL_MS = 5000;
 volatile uint16_t pulseCounter = 0;
 
 // === Simulação de GPS ===
 void simulateGpsMovement() {
   static float heading = 0.0f;
   heading += 0.3f;
   if (heading > 360.0f) heading -= 360.0f;
 
   const float distance = currentState.velocidade / 3600.0f; // km/s
   const float earthRadiusKm = 6371.0f;
   const float deltaLat = (distance / earthRadiusKm) * 57.2957795f;
   const float deltaLon = deltaLat / cos(currentState.latitude * 0.0174532925f);
 
   currentState.latitude  += deltaLat * cos(heading * 0.0174532925f);
   currentState.longitude += deltaLon * sin(heading * 0.0174532925f);
 }
 
 // === Leitura de sensores ===
 float readTemperatureC() {
 #if USE_REAL_TEMPERATURE
   temperatureSensor.requestTemperatures();
   float tempC = temperatureSensor.getTempCByIndex(0);
   if (tempC == DEVICE_DISCONNECTED_C) return NAN;
   return tempC;
 #else
   static float base = 75.0f;
   base += 0.5f * sin(millis() / 3000.0f);
   return base + random(-20, 20) / 10.0f;
 #endif
 }
 
 float readFuelLevelPercent() {
 #if USE_REAL_FUEL_LEVEL
   int raw = analogRead(PIN_FUEL_LEVEL);
   return map(raw, 0, 4095, 0, 100);
 #else
   static float level = currentState.nivelCombustivel;
   level -= currentState.velocidade * 0.0005f + 0.02f;
   if (level < 5.0f) {
     level = 5.0f + random(0, 100) / 100.0f;
   }
   return level;
 #endif
 }
 
 float readSpeedKmH() {
 #if USE_REAL_SPEED_SENSOR
   noInterrupts();
   uint16_t pulses = pulseCounter;
   pulseCounter = 0;
   interrupts();
 
   const float wheelCircumferenceM = 1.9f;
   const float pulsesPerRevolution = 2.0f;
   float revolutions     = pulses / pulsesPerRevolution;
   float distanceMeters  = revolutions * wheelCircumferenceM;
   float speedMps        = distanceMeters / (PUBLISH_INTERVAL_MS / 1000.0f);
   return speedMps * 3.6f;
 #else
   static float phase = 0.0f;
   phase += 0.08f;
   if (phase > TWO_PI) phase -= TWO_PI;
 
   float speed = 40.0f + 20.0f * sin(phase) + random(-30, 30) / 10.0f;
   if (speed < 0) speed = 0;
   return speed;
 #endif
 }
 
 float computeRpmFromSpeed(float speedKmH) {
   const float gearRatio          = 13.0f;
   const float tireCircumferenceM = 1.9f;
   float speedMps   = speedKmH / 3.6f;
   float wheelRps   = speedMps / tireCircumferenceM;
   float motorRps   = wheelRps * gearRatio;
   return motorRps * 60.0f;
 }
 
 void updateStatusFlags() {
   if (currentState.nivelCombustivel < 10.0f) {
     currentState.status       = "combustivel_baixo";
     currentState.observacoes  = "Alerta: combustível abaixo de 10%";
     digitalWrite(PIN_LED_ALERT, HIGH);
   } else if (currentState.temperaturaMotor > 95.0f) {
     currentState.status       = "superaquecimento";
     currentState.observacoes  = "Alerta: superaquecimento detectado";
     digitalWrite(PIN_LED_ALERT, HIGH);
   } else if (currentState.velocidade < 1.0f) {
     currentState.status       = "parada";
     currentState.observacoes  = "Moto parada";
     digitalWrite(PIN_LED_ALERT, LOW);
   } else {
     currentState.status       = "em_uso";
     currentState.observacoes  = "Moto em utilização normal";
     digitalWrite(PIN_LED_ALERT, LOW);
   }
 }
 
 void attachHallSensor() {
 #if USE_REAL_SPEED_SENSOR
   pinMode(PIN_SPEED_SENSOR, INPUT_PULLUP);
   attachInterrupt(
     digitalPinToInterrupt(PIN_SPEED_SENSOR),
     [](){ pulseCounter++; },
     RISING
   );
 #endif
 }
 
 // === Rede/MQTT ===
 void connectWifi() {
   Serial.print("Conectando ao Wi-Fi ");
   Serial.println(WIFI_SSID);
   WiFi.mode(WIFI_STA);
   WiFi.begin(WIFI_SSID, WIFI_PASSWORD, 6); // <-- canal 6 acelera a conexão no Wokwi
   while (WiFi.status() != WL_CONNECTED) {
     delay(100);
     Serial.print(".");
   }
   Serial.print("\nWi-Fi conectado. IP: ");
   Serial.println(WiFi.localIP());
 }
 
 void connectMqtt() {
   while (!mqttClient.connected()) {
     Serial.print("Conectando ao broker MQTT...");
     bool connected = false;
 
     if (MQTT_USERNAME && MQTT_PASSWORD_) {
       connected = mqttClient.connect(MQTT_CLIENT_ID, MQTT_USERNAME, MQTT_PASSWORD_);
     } else {
       connected = mqttClient.connect(MQTT_CLIENT_ID);
     }
 
     if (connected) {
       Serial.println(" conectado!");
     } else {
       Serial.print(" falhou, rc=");
       Serial.print(mqttClient.state());
       Serial.println(" -> tentando novamente em 5s");
       delay(5000);
     }
   }
 }
 
 // === Publicação da telemetria ===
 void publishTelemetry() {
   currentState.temperaturaMotor = readTemperatureC();
   currentState.nivelCombustivel = readFuelLevelPercent();
   currentState.velocidade       = readSpeedKmH();
   currentState.rotacaoMotor     = computeRpmFromSpeed(currentState.velocidade);
   currentState.quilometragem   += (currentState.velocidade / 3600.0f) * (PUBLISH_INTERVAL_MS / 1000.0f);
 
 #if USE_GPS_MODULE
   while (gpsSerial.available()) {
     gps.encode(gpsSerial.read());
   }
   if (gps.location.isValid()) {
     currentState.latitude  = gps.location.lat();
     currentState.longitude = gps.location.lng();
   }
 #else
   simulateGpsMovement();
 #endif
 
  updateStatusFlags();

  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("WiFi desconectado, não enviando telemetria.");
    return;
  }

  String payload = "{";
  payload += "\"deviceId\":" + String(currentState.deviceId) + ",";
  payload += "\"latitude\":" + String(currentState.latitude, 6) + ",";
  payload += "\"longitude\":" + String(currentState.longitude, 6) + ",";
  payload += "\"status\":\"" + currentState.status + "\",";
  payload += "\"temperatura\":" + String(currentState.temperaturaMotor, 2) + ",";
  payload += "\"velocidade\":" + String(currentState.velocidade, 2) + ",";
  payload += "\"quilometragem\":" + String(currentState.quilometragem, 2) + ",";
  payload += "\"nivelCombustivel\":" + String(currentState.nivelCombustivel, 2) + ",";
  payload += "\"rotacaoMotor\":" + String(currentState.rotacaoMotor, 2) + ",";
  payload += "\"observacoes\":\"" + currentState.observacoes + "\"";
  payload += "}";

  Serial.print("Publicando MQTT: ");
  Serial.println(payload);

  if (!mqttClient.publish(MQTT_TOPIC, payload.c_str())) {
    Serial.println("Falha ao publicar MQTT");
  }

#if USE_HTTP_BACKEND
  HTTPClient http;
  String url = String(API_BASE_URL) + "/telemetria";

  http.begin(url);
  http.addHeader("Content-Type", "application/json");

  int httpCode = http.POST(payload);

  if (httpCode > 0) {
    Serial.printf("Resposta API: %d\n", httpCode);
  } else {
    Serial.printf("Falha HTTP: %s\n", http.errorToString(httpCode).c_str());
  }

  http.end();
#endif
 }
 
 
 void setup() {
   Serial.begin(115200);
   delay(200);
   while (!Serial);          // segura até o monitor conectar
   Serial.println("Boot ESP32");
  secureClient.setCACert(HIVEMQ_CLOUD_CA_CERT);
  secureClient.setHandshakeTimeout(30);

   pinMode(PIN_LED_ALERT, OUTPUT);
   digitalWrite(PIN_LED_ALERT, LOW);
 
 #if USE_REAL_TEMPERATURE
   temperatureSensor.begin();
 #endif
 
 #if USE_GPS_MODULE
   gpsSerial.begin(9600, SERIAL_8N1, 16, 17);
 #endif
 
   attachHallSensor();
 
   connectWifi();
   mqttClient.setServer(MQTT_BROKER, MQTT_PORT);
   mqttClient.setBufferSize(512);
   Serial.printf("MQTT buffer: %d bytes\n", mqttClient.getBufferSize());
   connectMqtt();
 
   randomSeed(analogRead(0));
   currentState.observacoes = "ESP32 inicializado";
 }
 
 void loop() {
   if (!mqttClient.connected()) {
     connectMqtt();
   }
   mqttClient.loop();
 
   unsigned long now = millis();
   if (now - lastPublish >= PUBLISH_INTERVAL_MS) {
     lastPublish = now;
     publishTelemetry();
   }
 }
 