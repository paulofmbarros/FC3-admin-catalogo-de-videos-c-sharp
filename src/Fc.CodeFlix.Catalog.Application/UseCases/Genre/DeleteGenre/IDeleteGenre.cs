// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;

using MediatR;

public interface IDeleteGenre : IRequestHandler<DeleteGenreInput, Unit>
{
    
}