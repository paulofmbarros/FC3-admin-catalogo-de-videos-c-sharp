// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.ListGenre;

using MediatR;

public interface IListGenres : IRequestHandler<ListGenresInput, ListGenresOutput>
{
    
}