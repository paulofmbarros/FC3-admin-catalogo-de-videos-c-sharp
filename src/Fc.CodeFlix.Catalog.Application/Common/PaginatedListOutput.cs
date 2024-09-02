// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Common;

public abstract class PaginatedListOutput<TOutputItems>
{
    public int Page { get; set; }

    public int PerPage { get; set; }

    public int Total { get; set; }

    public IReadOnlyList<TOutputItems> Items { get; set; }

    public PaginatedListOutput(int page, int perPage, int total, IReadOnlyList<TOutputItems> items)
    {
        this.Page = page;
        this.PerPage = perPage;
        this.Total = total;
        this.Items = items;
    }
}