// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF;

using Fc.CodeFlix.Catalog.Application.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly CodeflixCatalogDbContext dbContext;

    public UnitOfWork(CodeflixCatalogDbContext dbContext) => this.dbContext = dbContext;

    public Task Commit(CancellationToken cancellationToken) => this.dbContext.SaveChangesAsync(cancellationToken);

    public Task Rollback(CancellationToken cancellationToken) => Task.CompletedTask;
}