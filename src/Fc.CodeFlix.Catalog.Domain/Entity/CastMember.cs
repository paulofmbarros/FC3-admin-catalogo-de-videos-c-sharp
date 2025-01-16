// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using Enum;
using SeedWork;
using Validation;

public class CastMember : AggregateRoot
{
    public string Name { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public CastMemberType Type { get; private set; }

    public CastMember(string name, CastMemberType type) : base()
    {
        this.Name = name;
        this.CreatedAt = DateTime.Now;
        this.Type = type;
        this.Validate();
    }

    private void Validate() => DomainValidation.NotNullOrEmpty(this.Name, nameof(this.Name));

    public void Update(string newName, CastMemberType newType)
    {
        this.Name = newName;
        this.Type = newType;
        this.Validate();
    }
}