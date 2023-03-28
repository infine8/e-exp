using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace EthExplorer.Service.Api.Helpers;

public static class AddSwaggerExtension
{
    public static void AddSwagger(this IServiceCollection services, SwaggerConfig swaggerConfig)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(swaggerConfig.Version, new OpenApiInfo { Title = swaggerConfig.Name, Version = swaggerConfig.Version });
        });
    }

    public static void UseSwagger(this IApplicationBuilder app, SwaggerConfig swaggerConfig)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint(swaggerConfig.Url, swaggerConfig.Name);
            c.DocExpansion(DocExpansion.None);
        });
    }
}