// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.Common;

using Domain.Entity;

public class GenreModelOutput
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public IReadOnlyList<Guid> Categories { get; set; }

    public GenreModelOutput(Guid id, string name, bool isActive, DateTime createdAt, IReadOnlyList<Guid> categories)
    {
        this.Id = id;
        this.Name = name;
        this.IsActive = isActive;
        this.CreatedAt = createdAt;
        this.Categories = categories;
    }

    public static GenreModelOutput FromGenre(Genre genre) => new (genre.Id, genre.Name, genre.IsActive, genre.CreatedAt, genre.Categories);
}