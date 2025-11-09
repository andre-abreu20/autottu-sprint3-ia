namespace AutoTTU.Configuration;

public class MqttOptions
{
    public string Broker { get; set; } = "b285d70f23b343eea183db3bb36fb891.s1.eu.hivemq.cloud";
    public int Port { get; set; } = 8883;
    public string Topic { get; set; } = "autottu/motos/1";
    public string ClientId { get; set; } = "autottu-api-consumer";
    public string? Username { get; set; } = "esp32-autottu";
    public string? Password { get; set; } = "##!U6d5tm3ZhpYB";
    public bool UseTls { get; set; } = true;
    public bool AllowUntrustedCertificates { get; set; } = false;
    public bool IgnoreCertificateChainErrors { get; set; } = false;
    public bool IgnoreCertificateRevocationErrors { get; set; } = false;
    public string? CaCertificatePath { get; set; }
}

