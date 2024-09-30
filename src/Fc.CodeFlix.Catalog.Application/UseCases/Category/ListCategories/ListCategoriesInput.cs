// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;

using Application.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;

public class ListCategoriesInput : PaginatedListInput, IRequest<ListCategoriesOutput>
{
    public ListCategoriesInput(
        int page = 1,
        int perPage = 15,
        string search = "",
        string sort = "Name",
        SearchOrder direction = SearchOrder.Asc ) : base(page, perPage, search, sort, direction)
    {
    }


    public ListCategoriesInput() : base(1,15,"", "", SearchOrder.Asc)
    {

    }
}