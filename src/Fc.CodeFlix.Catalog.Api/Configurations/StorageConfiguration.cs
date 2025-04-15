// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Configurations;

using Application.Interfaces;
using Google.Cloud.Storage.V1;
using Infra.Storage.Configuration;
using Infra.Storage.Service;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorageConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(_ => StorageClient.Create());
        services.Configure<StorageServiceOptions>(configuration.GetSection(StorageServiceOptions.ConfigurationSection));
        services.AddTransient<IStorageService, StorageService>();

        return services;
    }
}
