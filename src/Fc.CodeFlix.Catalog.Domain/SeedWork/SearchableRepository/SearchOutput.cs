// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

public class SearchOutput<TAggregate> where TAggregate : AggregateRoot
{
    public int CurrentPage { get; set; }

    public int PerPage { get; set; }

    public int Total { get; set; }

    public IReadOnlyList<TAggregate> Items { get; set; }

    public SearchOutput(int currentPage, int perPage, int total, IReadOnlyList<TAggregate> items)
    {
        this.CurrentPage = currentPage;
        this.PerPage = perPage;
        this.Total = total;
        this.Items = items;
    }
}