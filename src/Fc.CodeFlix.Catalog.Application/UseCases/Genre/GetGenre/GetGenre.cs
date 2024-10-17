// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;

using Common;
using Domain.Repository;

public class GetGenre : IGetGenre
{
    private readonly IGenreRepository genreRepository;

    public GetGenre(IGenreRepository genreRepository) => this.genreRepository = genreRepository;

    public async Task<GenreModelOutput> Handle(GetGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await this.genreRepository.Get(request.Id, cancellationToken);

        return GenreModelOutput.FromGenre(genre);
    }
}