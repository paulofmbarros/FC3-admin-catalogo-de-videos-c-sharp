// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.ApiModels.Category;

public class UpdateCategoryApiInput(string name, string? description = null, bool? isActive = null)
{
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description;
    public bool? IsActive { get; set; } = isActive;
}