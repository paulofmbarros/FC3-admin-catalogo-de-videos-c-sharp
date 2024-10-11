// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;

using Common;
using MediatR;

public class UpdateGenreInput : IRequest<GenreModelOutput>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool? IsActive { get; set; }

    public UpdateGenreInput(Guid id, string name, bool? isActive)
    {
        this.Id = id;
        this.Name = name;
        this.IsActive = isActive;
    }
}