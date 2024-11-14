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
    private readonly ICategoryRepository categoryRepository;

    public ListGenres(IGenreRepository genreRepository, ICategoryRepository categoryRepository)
    {
        this.genreRepository = genreRepository;
        this.categoryRepository = categoryRepository;
    }

    public async Task<ListGenresOutput> Handle(ListGenresInput request, CancellationToken cancellationToken)
    {
        var searchInput = request.ToSearchInput();
        var searchOutput = await this.genreRepository.Search(searchInput, cancellationToken);

        var relatedCategoriesIds = searchOutput.Items
            .SelectMany(item => item.Categories)
            .Distinct()
            .ToList();

        var output = ListGenresOutput.FromSearchOutput(searchOutput);

        if (relatedCategoriesIds.Count == 0)
        {
            return output;
        }

        var categories = await this.categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);

        output.FillWithCategoryNames(categories);

        return output;

    }
}