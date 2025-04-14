// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork;

public abstract class AggregateRoot : Entity
{
    private List<DomainEvent> events = new();
    public IReadOnlyCollection<DomainEvent> Events => events.AsReadOnly();

    protected AggregateRoot() : base() { }

    public void RaiseEvent(DomainEvent @event) => this.events.Add(@event);
    public void ClearEvents() => this.events.Clear();
}