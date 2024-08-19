// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using Exceptions;
using SeedWork;
using Validation;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool? isActive = true) : base()
    {
        this.Name = name;
        this.Description = description;
        this.IsActive = true;
        this.CreatedAt = DateTime.Now;
        this.IsActive = isActive ?? true;

        this.Validate();
    }

    public void Activate()
    {
        this.IsActive = true;
        this.Validate();
    }

    public void Deactivate()
    {
        this.IsActive = false;
        this.Validate();
    }

    public void Update(string name, string? description = null)
    {
        this.Name = name;
        this.Description = description ?? this.Description;
        this.Validate();
    }


    private void Validate()
    {

        DomainValidation.NotNullOrEmpty(this.Name, nameof(Name));

        DomainValidation.MinLength(this.Name, 3, nameof(Name));

        DomainValidation.MaxLength(this.Name, 255, nameof(Name));

        DomainValidation.NotNull(this.Description, nameof(Description));

        DomainValidation.MaxLength(this.Description, 10_000, nameof(Description));

    }
}