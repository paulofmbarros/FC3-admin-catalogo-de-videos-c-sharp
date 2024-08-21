// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;

using Domain.Entity;

public class CategoryModelOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public CategoryModelOutput(Guid id, string name, string description, bool isActive, DateTime createdAt)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.IsActive = isActive;
        this.CreatedAt = createdAt;
    }

    public static CategoryModelOutput FromCategory(Category category) =>
        new(category.Id, category.Name, category.Description, category.IsActive, category.CreatedAt);
}