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
        new(searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items.Select(GenreModelOutput.FromGenre)
                .ToList());

    public void FillWithCategoryNames(IReadOnlyList<Category> categories)
    {
        foreach (var item in this.Items)
        {
            foreach (var categoryOutput in item.Categories)
            {
                var category = categories?.FirstOrDefault(c => c.Id == categoryOutput.Id);
                if (category != null)
                {
                    categoryOutput.Name = category.Name;
                }
            }
        }
    }
}