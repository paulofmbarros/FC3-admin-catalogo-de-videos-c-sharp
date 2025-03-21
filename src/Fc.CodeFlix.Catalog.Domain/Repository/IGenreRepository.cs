﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Repository;

using Entity;
using SeedWork;
using SeedWork.SearchableRepository;

public interface IGenreRepository : IGenericRepository<Genre>, ISearchableRepository<Genre>
{
    public Task<IReadOnlyList<Guid>> GetIdsByIds(List<Guid> ids, CancellationToken cancellationToken);

    public Task<IReadOnlyList<Genre>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken);

}