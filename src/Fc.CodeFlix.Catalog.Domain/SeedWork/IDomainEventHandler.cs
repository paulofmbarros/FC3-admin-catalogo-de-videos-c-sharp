﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork;

public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : DomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
}
