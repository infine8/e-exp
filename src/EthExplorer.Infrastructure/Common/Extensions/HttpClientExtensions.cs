using EthExplorer.Domain.Common.Extensions;
using Polly;
using Polly.Extensions.Http;

namespace EthExplorer.Infrastructure.Common.Extensions;

public static class HttpClientExtensions
{
    public static void ConfigureClient(this IHttpClientBuilder builder, int maxConnections = 20, double timeoutSec = 30, double lifeTimeMin = 5, bool addRetryPolicy = true)
    {
        builder.ConfigureClient(null, maxConnections, timeoutSec, lifeTimeMin, addRetryPolicy);
    }

    public static void ConfigureClient(this IHttpClientBuilder builder, string serviceUrl, int maxConnections = 20, double timeoutSec = 60, double lifeTimeMin = 5, bool addRetryPolicy = true)
    {
        builder.ConfigureHttpClient(hc =>
            {
                hc.Timeout = TimeSpan.FromSeconds(timeoutSec);
                if (!serviceUrl.IsNullOrEmpty()) hc.BaseAddress = new Uri(serviceUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                MaxConnectionsPerServer = maxConnections,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(lifeTimeMin));

        if (addRetryPolicy)
            builder.AddRetryPolicy();
    }

    private static void AddRetryPolicy(this IHttpClientBuilder builder, int retryCount = 3)
    {
        builder.AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
    }
}