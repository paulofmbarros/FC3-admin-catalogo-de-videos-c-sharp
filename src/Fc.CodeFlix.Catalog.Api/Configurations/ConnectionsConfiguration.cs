// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Configurations;

using FC.CodeFlix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConections(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbConnection(configuration);
        return services;
    }
    private static IServiceCollection AddDbConnection(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDb");
        services.AddDbContext<CodeflixCatalogDbContext>(
            options => options.UseMySql(
                connectionString, ServerVersion.AutoDetect(connectionString)
            )
        );
        return services;
    }
}