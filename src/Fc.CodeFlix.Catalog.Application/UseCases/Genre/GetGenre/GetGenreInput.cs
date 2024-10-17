// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;

using Common;
using MediatR;

public class GetGenreInput : IRequest<GenreModelOutput>
{
    public Guid Id { get; set; }

    public GetGenreInput(Guid id) => this.Id = id;
}