// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using SeedWork;
using Validation;

public class Genre
{
    public string Name { get; private set; }
    
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private List<Guid> categories;
    public IReadOnlyList<Guid> Categories => this.categories.AsReadOnly();

    public Genre(string name, bool isActive = true)
    {
        this.Name = name;
        this.IsActive = isActive;
        this.CreatedAt = DateTime.Now;
        this.categories = new List<Guid>();
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

    public void AddCategory(Guid categoryId)
    {
        this.categories.Add(categoryId);

        this.Validate();
    }

    public void RemoveCategory(Guid categoryId)
    {
        this.categories.Remove(categoryId);

        this.Validate();
    }

    public void RemoveAllCategories()
    {
       this.categories.Clear();
       this.Validate();
    }
}