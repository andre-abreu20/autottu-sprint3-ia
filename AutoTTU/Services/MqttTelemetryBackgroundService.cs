using System;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
using AutoTTU.Configuration;
using AutoTTU.Connection;
using AutoTTU.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;

namespace AutoTTU.Services;

public class MqttTelemetryBackgroundService : BackgroundService
{
    private readonly ILogger<MqttTelemetryBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MqttOptions _mqttOptions;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private IMqttClient? _mqttClient;

    public MqttTelemetryBackgroundService(
        ILogger<MqttTelemetryBackgroundService> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<MqttOptions> mqttOptions)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _mqttOptions = mqttOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        _mqttClient.ApplicationMessageReceivedAsync += args =>
            HandleIncomingMessageAsync(args, stoppingToken);

        _mqttClient.DisconnectedAsync += async args =>
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            _logger.LogWarning("Conexão MQTT perdida: {Reason}", args.ReasonString ?? args.Exception?.Message);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            await ConnectAndSubscribeAsync(stoppingToken);
        };

        await ConnectAndSubscribeAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Ignorado — cancelamento esperado
        }
    }

    private async Task ConnectAndSubscribeAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient is null)
        {
            throw new InvalidOperationException("Cliente MQTT não inicializado.");
        }

        var clientId = string.IsNullOrWhiteSpace(_mqttOptions.ClientId)
            ? $"autottu-api-{Guid.NewGuid():N}"
            : _mqttOptions.ClientId;

        var broker = string.IsNullOrWhiteSpace(_mqttOptions.Broker)
            ? "broker.hivemq.com"
            : _mqttOptions.Broker;

        var port = _mqttOptions.Port <= 0 ? 1883 : _mqttOptions.Port;
        var topic = string.IsNullOrWhiteSpace(_mqttOptions.Topic)
            ? "autottu/motos/1"
            : _mqttOptions.Topic;

        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(broker, port);

        if (!string.IsNullOrWhiteSpace(_mqttOptions.Username) &&
            !string.IsNullOrWhiteSpace(_mqttOptions.Password))
        {
            optionsBuilder.WithCredentials(_mqttOptions.Username, _mqttOptions.Password);
        }

        if (_mqttOptions.UseTls)
        {
            optionsBuilder.WithTls(o =>
            {
                o.UseTls = true;
                o.AllowUntrustedCertificates = _mqttOptions.AllowUntrustedCertificates;
                o.IgnoreCertificateChainErrors = _mqttOptions.IgnoreCertificateChainErrors;
                o.IgnoreCertificateRevocationErrors = _mqttOptions.IgnoreCertificateRevocationErrors;

                if (!string.IsNullOrWhiteSpace(_mqttOptions.CaCertificatePath) &&
                    File.Exists(_mqttOptions.CaCertificatePath))
                {
                    var caCertificate = new X509Certificate2(File.ReadAllBytes(_mqttOptions.CaCertificatePath));
                    o.Certificates = new List<X509Certificate> { caCertificate };
                }
            });
        }

        var options = optionsBuilder.Build();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _mqttClient.ConnectAsync(options, cancellationToken);
                _logger.LogInformation("Conectado ao broker MQTT {Broker}:{Port} com ClientId {ClientId}", broker, port, clientId);

                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .WithAtLeastOnceQoS()
                    .Build(), cancellationToken);

                _logger.LogInformation("Inscrito no tópico MQTT: {Topic}", topic);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao conectar/inscrever no broker MQTT. Tentando novamente em 5 segundos...");
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }

    private async Task HandleIncomingMessageAsync(MqttApplicationMessageReceivedEventArgs args, CancellationToken cancellationToken)
    {
        try
        {
            var payloadString = args.ApplicationMessage.PayloadSegment.Count > 0
                ? Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment)
                : string.Empty;

            if (string.IsNullOrWhiteSpace(payloadString))
            {
                _logger.LogWarning("Mensagem MQTT recebida sem payload. Ignorando.");
                return;
            }

            var telemetryMessage = JsonSerializer.Deserialize<TelemetryMessage>(payloadString, _jsonOptions);

            if (telemetryMessage is null)
            {
                _logger.LogWarning("Não foi possível desserializar a mensagem MQTT: {Payload}", payloadString);
                return;
            }

            await PersistTelemetryAsync(telemetryMessage, cancellationToken);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "Erro ao desserializar payload MQTT.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem MQTT.");
        }
    }

    private async Task PersistTelemetryAsync(TelemetryMessage message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var moto = await dbContext.Motos
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.IdMoto == message.DeviceId, cancellationToken);

        if (moto is null)
        {
            _logger.LogWarning("Mensagem MQTT ignorada: dispositivo {DeviceId} não cadastrado.", message.DeviceId);
            return;
        }

        var telemetria = new Telemetria
        {
            DeviceId = message.DeviceId,
            Timestamp = DateTime.UtcNow,
            Latitude = Convert.ToDecimal(message.Latitude),
            Longitude = Convert.ToDecimal(message.Longitude),
            Status = message.Status ?? "desconhecido",
            Temperatura = message.Temperatura is null ? null : Convert.ToDecimal(message.Temperatura),
            Velocidade = message.Velocidade is null ? null : Convert.ToDecimal(message.Velocidade),
            Quilometragem = message.Quilometragem is null ? null : Convert.ToDecimal(message.Quilometragem),
            NivelCombustivel = message.NivelCombustivel is null ? null : Convert.ToDecimal(message.NivelCombustivel),
            RotacaoMotor = message.RotacaoMotor is null ? null : Convert.ToDecimal(message.RotacaoMotor),
            Observacoes = message.Observacoes
        };

        dbContext.Telemetria.Add(telemetria);
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Telemetria salva para DeviceId {DeviceId} às {Timestamp}.", message.DeviceId, telemetria.Timestamp);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient is not null && _mqttClient.IsConnected)
        {
            try
            {
                await _mqttClient.DisconnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao desconectar do broker MQTT durante o encerramento.");
            }
        }

        await base.StopAsync(cancellationToken);
    }

    private sealed record TelemetryMessage(
        int DeviceId,
        double Latitude,
        double Longitude,
        string? Status,
        double? Temperatura,
        double? Velocidade,
        double? Quilometragem,
        double? NivelCombustivel,
        double? RotacaoMotor,
        string? Observacoes);
}

