// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;

using Common;
using MediatR;

public interface IUpdateGenre : IRequestHandler<UpdateGenreInput,GenreModelOutput>
{
    
}