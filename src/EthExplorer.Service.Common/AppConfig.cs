namespace EthExplorer.Service.Common;

public record AppConfig
{
    public IConfiguration SourceConfiguration { get; set; }
    
    public string HealthcheckPath { get; set; }
    
    public HttpsConfig Https { get; set; }
    public string ApiServiceUrl { get; set; }
}

public record HttpsConfig
{
    public int Port { get; set; }
    public CertConfig Cert { get; set; }
}

public record CertConfig
{
    public string Base64 { get; set; }
    public string Password { get; set; }
}