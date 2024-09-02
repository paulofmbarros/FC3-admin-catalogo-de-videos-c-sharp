// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

public class SearchInput
{
    public int Page { get; set; }

    public int PerPage { get; set; }

    public string Search { get; set; }

    public string OrderBy { get; set; }

    public SearchOrder Order { get; set; }

    public SearchInput(int page, int perPage, string search, string orderBy, SearchOrder order)
    {
        this.Page = page;
        this.PerPage = perPage;
        this.Search = search;
        this.OrderBy = orderBy;
        this.Order = order;
    }
}