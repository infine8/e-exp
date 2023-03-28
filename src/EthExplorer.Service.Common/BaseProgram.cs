using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using EthExplorer.Infrastructure;
using EthExplorer.Infrastructure.Bootstrap;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Formatting.Elasticsearch;

namespace EthExplorer.Service.Common;

public abstract class BaseProgram
{
    protected static async Task RunApp(IHostBuilder hostBuilder)
    {
        try
        {
            await hostBuilder.Build().RunAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    protected static IHostBuilder CreateHostBuilder<TConfig>(Assembly executingAssembly, string[] args,
        Action<IServiceCollection, TConfig>? addServicesFunc = null,
        Action<IApplicationBuilder, TConfig>? useServicesFunc = null,
        Action<IEndpointRouteBuilder>? buildCustomRoutes = null,
        Action<IWebHostBuilder, TConfig>? buildWebHost = null) where TConfig : AppConfig, new() =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                Console.WriteLine($"ASPNETCORE_URLS: {Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}");

                if (AppEnvironment.CurrentEnv == AppEnvironment.AppEnvironmentType.Undefined)
                {
                    var ex = new ApplicationException("EnvVar 'ASPNETCORE_ENVIRONMENT' is not defined");

                    Console.WriteLine(ex);
                    throw ex;
                }

                var config = AppConfigHelper.ConfigureAppConfig<TConfig>(executingAssembly);

                webBuilder
                    .ConfigureAppConfiguration(builder =>
                    {
                        builder.AddConfiguration(config.SourceConfiguration);
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient<ExceptionHandlingMiddleware>();

                        services.AddHealthChecks();

                        services.AddWeb3();
                        services.AddCoreServices(config.ApiServiceUrl);
                        services.AddDbContexts();
                        services.AddAggregates();

                        services
                            .AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
                            .AddApplicationPart(executingAssembly)
                            .AddJsonOptions(options =>
                            {
                                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                options.JsonSerializerOptions.WriteIndented = true;
                                options.JsonSerializerOptions.Converters.Add(new DecimalConverter());
                            });

                        addServicesFunc?.Invoke(services, config);
                    })
                    .Configure(app =>
                    {
                        app.UseMiddleware<ExceptionHandlingMiddleware>();

                        app.UseRouting();

                        app.UseCloudEvents();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapSubscribeHandler();
                            endpoints.MapHealthChecks(config.HealthcheckPath, new HealthCheckOptions
                            {
                                Predicate = _ => true,
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                            });

                            buildCustomRoutes?.Invoke(endpoints);
                        });

                        useServicesFunc?.Invoke(app, config);
                    })
                    .ConfigureLogging(builder => { builder.ClearProviders(); });

#if !DEBUG
                webBuilder.ConfigureKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 4194304;
                    options.ConfigureHttpsDefaults (httpsOptions => httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13);
                    
                    options.ListenAnyIP(config.Https.Port, (httpsOptions =>
                    {
                        httpsOptions.UseHttps(new X509Certificate2(Convert.FromBase64String(config.Https.Cert.Base64), config.Https.Cert.Password));
                        httpsOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    }));
                });
#endif
                buildWebHost?.Invoke(webBuilder, config);
            })
            .UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
                
                if (AppEnvironment.IsLocal)
                {
                    config.WriteTo.Console();
                }
                else
                {
                    config.WriteTo.Console(new ElasticsearchJsonFormatter());
                }
            });
}