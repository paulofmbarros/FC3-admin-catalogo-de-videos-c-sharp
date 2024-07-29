// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using Exceptions;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string name, string description, bool isActive = true)
    {
        this.Id = Guid.NewGuid();
        this.Name = name;
        this.Description = description;
        this.IsActive = true;
        this.CreatedAt = DateTime.Now;
        this.IsActive = isActive;

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
        if (string.IsNullOrWhiteSpace(this.Name))
        {
            throw new EntityValidationException($"{nameof(this.Name)} should not be empty or null");
        }

        if (this.Description is null)
        {
            throw new EntityValidationException($"{nameof(this.Description)} should not be empty or null");
        }

        if(this.Name.Length<3)
        {
            throw new EntityValidationException($"{nameof(this.Name)} should have at least 3 characters");
        }

        if(this.Name.Length>255)
        {
            throw new EntityValidationException($"{nameof(this.Name)} should have less than 255 characters");
        }

        if(this.Description.Length>10_000)
        {
            throw new EntityValidationException($"{nameof(this.Description)} should have less than 10.000 characters");
        }
    }
}