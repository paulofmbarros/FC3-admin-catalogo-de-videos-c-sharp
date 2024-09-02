// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;

using Application.Common;
using Common;

public class ListCategoriesOutput : PaginatedListOutput<CategoryModelOutput>
{
    public ListCategoriesOutput(int page,
        int perPage,
        int total,
        IReadOnlyList<CategoryModelOutput> items) : base(page, perPage, total, items)
    {
    }
}