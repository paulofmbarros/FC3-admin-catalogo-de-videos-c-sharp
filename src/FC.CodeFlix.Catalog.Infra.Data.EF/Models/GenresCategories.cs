// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Models;

using Fc.CodeFlix.Catalog.Domain.Entity;

public class GenresCategories
{
    public Category? Category { get; set; }

    public Guid CategoryId { get; set; }

    public Genre? Genre { get; set; }

    public Guid GenreId { get; set; }

    public GenresCategories(Guid categoryId, Guid genreId)
    {
        this.CategoryId = categoryId;
        this.GenreId = genreId;
    }
}