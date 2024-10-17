// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;

using MediatR;

public class DeleteGenreInput : IRequest<Unit>
{
    public DeleteGenreInput(Guid id) => this.Id = id;
    public Guid Id { get; set; }
}