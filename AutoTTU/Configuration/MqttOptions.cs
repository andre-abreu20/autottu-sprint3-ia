namespace AutoTTU.Configuration;

public class MqttOptions
{
    public string Broker { get; set; } = "broker.hivemq.com";
    public int Port { get; set; } = 1883;
    public string Topic { get; set; } = "autottu/motos/1";
    public string? ClientId { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool UseTls { get; set; }
    public bool AllowUntrustedCertificates { get; set; }
    public bool IgnoreCertificateChainErrors { get; set; }
    public bool IgnoreCertificateRevocationErrors { get; set; }
    public string? CaCertificatePath { get; set; }
}
