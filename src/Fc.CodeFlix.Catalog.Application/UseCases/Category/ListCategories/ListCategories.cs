// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;

using Common;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;

public class ListCategories : IListCategories
{
    private readonly ICategoryRepository categoryRepository;

    public ListCategories(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<ListCategoriesOutput> Handle(ListCategoriesInput request, CancellationToken cancellationToken)
    {
        var searchOutput = await categoryRepository
            .Search(
                new SearchInput(request.Page,
                    request.PerPage,
                    request.Search,
                    request.Sort,
                    request.Direction),
                cancellationToken);

        return new ListCategoriesOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(x => CategoryModelOutput.FromCategory(x))
                .ToList());

    }
}