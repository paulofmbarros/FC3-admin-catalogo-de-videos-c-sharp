// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

public interface ISearchableRepository<TAggregate> where TAggregate : AggregateRoot
{
    Task<SearchOutput<TAggregate>> Search(SearchInput searchInput, CancellationToken cancellationToken);
}