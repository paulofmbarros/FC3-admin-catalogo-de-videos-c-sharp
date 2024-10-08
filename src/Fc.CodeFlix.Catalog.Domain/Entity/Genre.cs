// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using Validation;

public class Genre
{
    public string Name { get; private set; }
    
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Genre(string name, bool isActive = true)
    {
        this.Name = name;
        this.IsActive = isActive;
        this.CreatedAt = DateTime.Now;
        this.Validate();

    }

    public void Deactivate()
    {
        this.IsActive = false;
        this.Validate();
    }

    public void Update(string newName)
    {
        this.Name = newName;
        this.Validate();
    }

    public void Activate()
    {
        this.IsActive = true;
        this.Validate();
    }

    private void Validate() => DomainValidation.NotNullOrEmpty(this.Name, nameof(this.Name));
}