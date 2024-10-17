// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;

using Category.Common;
using Common;
using MediatR;

public interface IGetGenre : IRequestHandler<GetGenreInput, GenreModelOutput>
{
    
}