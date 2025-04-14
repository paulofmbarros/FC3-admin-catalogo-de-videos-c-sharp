// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork;

public interface IDomainEventPublisher
{
    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken)
        where TDomainEvent : DomainEvent;
}
