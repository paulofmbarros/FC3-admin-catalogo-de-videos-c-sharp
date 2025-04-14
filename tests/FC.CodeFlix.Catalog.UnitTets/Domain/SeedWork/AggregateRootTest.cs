// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.SeedWork;

using FluentAssertions;

public class AggregateRootTest
{
    [Fact(DisplayName = nameof(RaiseEvent))]
    [Trait("Domain", "AggregateRoot")]
    public void RaiseEvent()
    {
        var domainEvent = new DomainEventFake();
        var aggregateRoot = new AggregateRootFake();

        aggregateRoot.RaiseEvent(domainEvent);

        aggregateRoot.Events.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(ClearEvents))]
    [Trait("Domain", "AggregateRoot")]
    public void ClearEvents()
    {
        var domainEvent = new DomainEventFake();
        var aggregateRoot = new AggregateRootFake();
        aggregateRoot.RaiseEvent(domainEvent);

        aggregateRoot.ClearEvents();

        aggregateRoot.Events.Should().HaveCount(0);
    }
}