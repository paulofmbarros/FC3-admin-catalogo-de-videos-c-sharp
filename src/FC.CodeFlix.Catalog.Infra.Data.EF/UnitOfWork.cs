// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF;

using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Domain.SeedWork;
using Microsoft.Extensions.Logging;

public class UnitOfWork : IUnitOfWork
{
    private readonly CodeflixCatalogDbContext dbContext;
    private readonly IDomainEventPublisher publisher;
    private readonly ILogger<UnitOfWork> logger;

    public UnitOfWork(CodeflixCatalogDbContext dbContext, IDomainEventPublisher publisher, ILogger<UnitOfWork> logger)
    {
        this.dbContext = dbContext;
        this.publisher = publisher;
        this.logger = logger;
    }

    public async Task Commit(CancellationToken cancellationToken)
    {
        var aggregateRoots = this.dbContext.ChangeTracker.Entries<AggregateRoot>()
            .Where(entry => entry.Entity.Events.Any())
            .Select(entry => entry.Entity)
            .ToList();

        this.logger.LogInformation($"Commit: {aggregateRoots.Count} aggregateRoots with events");

        var events = aggregateRoots.SelectMany(entity => entity.Events);
        this.logger.LogInformation($"Commit: {events.Count()} events raised");

        foreach (var @event in events)
        {
            await this.publisher.PublishAsync((dynamic)@event, cancellationToken);
        }

        foreach (var entity in aggregateRoots)
        {
             entity.ClearEvents();
        }

       await this.dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task Rollback(CancellationToken cancellationToken) => Task.CompletedTask;
}
