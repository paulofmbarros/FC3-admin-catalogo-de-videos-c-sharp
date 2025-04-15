// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Configurations;

using Application;
using Application.EventHandlers;
using Application.Interfaces;
using Application.UseCases.Category.CreateCategory;
using Domain.Events;
using Domain.Repository;
using Domain.SeedWork;
using FC.CodeFlix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Infra.Messaging.Configuration;
using FC.CodeFlix.Catalog.Infra.Messaging.Producer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public static class UseCasesConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCategory>());
        services.AddRepositories();
        services.AddDomainEvents(configuration);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IGenreRepository, GenreRepository>();
        services.AddTransient<ICastMemberRepository, CastMemberRepository>();
        services.AddTransient<IVideoRepository, VideoRepository>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddDomainEvents(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
        services.AddTransient<IDomainEventHandler<VideoUploadedEvent>, SendToEncodedEventHandler>();

        services.Configure<RabbitMqConfiguration>(configuration.GetSection(RabbitMqConfiguration.ConfigurationSection));
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;
            var factory = new ConnectionFactory
            {
                HostName = config.Hostname,
                UserName = config.Username,
                Password = config.Password

            };

            return factory.CreateConnection();
        });

        services.AddSingleton<ChannelManager>();
        services.AddTransient<IMessageProducer>(sp =>
        {
            var channelManager = sp.GetRequiredService<ChannelManager>();
            var config = sp.GetRequiredService<IOptions<RabbitMqConfiguration>>();

            return new RabbitMqProducer(channelManager.GetChannel(), config);
        });

        return services;
    }
}
