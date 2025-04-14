// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application;

using Domain.SeedWork;
using Microsoft.Extensions.DependencyInjection;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IServiceProvider serviceProvider;

    public DomainEventPublisher(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

    public async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : DomainEvent
    {
        var handlers = this.serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();

        if (!handlers.Any())
        {
            return;
        }

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(domainEvent, cancellationToken);
        }
    }
}
