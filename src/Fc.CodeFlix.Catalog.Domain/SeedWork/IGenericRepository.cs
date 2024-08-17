// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.SeedWork;

public interface IGenericRepository<TAggregate> : IRepository
{
    public Task Insert(TAggregate aggregate, CancellationToken cancellationToken);

}