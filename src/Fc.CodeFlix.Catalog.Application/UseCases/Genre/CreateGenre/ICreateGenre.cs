// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;

using Common;
using MediatR;

public interface ICreateGenre : IRequestHandler<CreateGenreInput, GenreModelOutput>
{


}