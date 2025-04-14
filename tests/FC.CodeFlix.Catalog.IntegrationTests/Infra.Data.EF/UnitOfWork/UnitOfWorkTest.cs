// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork;

using Catalog.Infra.Data.EF;
using Fc.CodeFlix.Catalog.Application;
using Fc.CodeFlix.Catalog.Domain.SeedWork;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest
{

    private readonly UnitOfWorkTestFixture fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCategoryList = this.fixture.GetExampleCategoriesList();
        var categoryWithEvent = exampleCategoryList.First();
        var @event = new DomainEventFake();
        categoryWithEvent.RaiseEvent(@event);
        var eventHandlerMock = new Mock<IDomainEventHandler<DomainEventFake>>();
        await dbContext.AddRangeAsync(exampleCategoryList);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddSingleton(eventHandlerMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);

        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());

        await unitOfWork.Commit(CancellationToken.None);

        var assertDbContext = this.fixture.CreateDbContext(true);
        var categories = await assertDbContext.Categories.AsNoTracking().ToListAsync();

        categories.Should().HaveCount(exampleCategoryList.Count);
        eventHandlerMock.Verify(handler => handler.HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
        categoryWithEvent.Events.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = this.fixture.CreateDbContext();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());


        var task = async () => await unitOfWork.Rollback(CancellationToken.None);

        await task.Should().NotThrowAsync();
    }


}
