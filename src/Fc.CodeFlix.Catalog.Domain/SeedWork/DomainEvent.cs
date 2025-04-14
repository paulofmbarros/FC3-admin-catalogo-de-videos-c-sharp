// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; set; }

    protected DomainEvent()
    {
        this.OccurredOn = DateTime.UtcNow;
    }
}