// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Common;

using Domain.SeedWork.SearchableRepository;

public abstract class PaginatedListInput
{
    public int Page { get; set; }

    public int PerPage { get; set; }

    public string Search { get; set; }

    public string Sort { get; set; }

    public SearchOrder Direction { get; set; }

    public PaginatedListInput(int page, int perPage, string search, string sort, SearchOrder direction)
    {
        this.Page = page;
        this.PerPage = perPage;
        this.Search = search;
        this.Sort = sort;
        this.Direction = direction;
    }
}