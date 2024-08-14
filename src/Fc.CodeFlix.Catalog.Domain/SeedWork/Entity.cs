// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork;

public abstract class Entity
{

    public Guid Id { get; protected set; }

    protected Entity()
    {
        this.Id = Guid.NewGuid();
    }
}