using EthExplorer.Service.Common;

namespace EthExplorer.Service.Api;

public record ApiConfig : AppConfig
{
    public SwaggerConfig Swagger { get; set; }
}

public record SwaggerConfig
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string Url { get; set; }
}