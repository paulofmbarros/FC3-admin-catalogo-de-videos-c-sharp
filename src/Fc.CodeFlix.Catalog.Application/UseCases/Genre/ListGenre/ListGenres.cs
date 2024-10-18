// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.ListGenre;

using Common;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;

public class ListGenres : IListGenres
{
    private readonly IGenreRepository genreRepository;

    public ListGenres(IGenreRepository genreRepository)
    {
        this.genreRepository = genreRepository;
    }

    public async Task<ListGenresOutput> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var searchInput = request.ToSearchInput();
        var searchOutput = await this.genreRepository.Search(searchInput, cancellationToken);

        return ListGenresOutput.FromSearchOutput(searchOutput);

    }
}