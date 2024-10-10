// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;

using Common;
using MediatR;

public class CreateGenreInput : IRequest<GenreModelOutput>
{
    public string Name { get; set; }

    public bool IsActive { get; set; }

    public List<Guid>? Categories { get; set; }

    public CreateGenreInput(string name, bool isActive, List<Guid>? categories = null)
    {
        this.Name = name;
        this.IsActive = isActive;
        this.Categories = categories;
    }
}