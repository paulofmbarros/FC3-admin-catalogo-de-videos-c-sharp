// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.ApiModels.Genre;

public class UpdateGenreApiInput(string name, bool? isActive = null, List<Guid>? categoriesIds = null)
{
    public string Name { get; set; } = name;
    public bool? IsActive { get; set; } = isActive;

    public List<Guid>? CategoriesIds { get; set; } = categoriesIds;
}