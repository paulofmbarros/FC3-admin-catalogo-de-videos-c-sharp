// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.ListGenre;

using Application.Common;
using Common;
using Domain.Entity;
using Domain.SeedWork.SearchableRepository;

public class ListGenresOutput(int page, int perPage, int total, IReadOnlyList<GenreModelOutput> items)
    : PaginatedListOutput<GenreModelOutput>(page, perPage, total, items)
{
    public static ListGenresOutput FromSearchOutput(SearchOutput<Genre> searchOutput) =>
        new(searchOutput.CurrentPage, searchOutput.PerPage, searchOutput.Total,
            searchOutput.Items.Select(GenreModelOutput.FromGenre).ToList());
}