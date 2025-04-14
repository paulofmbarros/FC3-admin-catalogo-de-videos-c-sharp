// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Configurations;

using System.Text.Json;
using FC.CodeFlix.Catalog.Infra.Messaging.JsonPolicies;
using Filters;

public static class ControllersConfiguration
{
    public static IServiceCollection AddAndConfigureControllers(
        this IServiceCollection services
    )
    {
        services
            .AddControllers(options => options.Filters.Add(typeof(ApiGlobalExceptionFilter)))
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCasePolicy();
            });

        services.AddDocumentation();
        return services;
    }
    private static IServiceCollection AddDocumentation(
        this IServiceCollection services
    )
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
    public static WebApplication UseDocumentation(
        this WebApplication app
    ){
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }
}
