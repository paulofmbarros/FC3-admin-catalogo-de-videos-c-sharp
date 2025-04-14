// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Integration/Application ", "Delete Category - Use Case")]
    public async Task DeleteCategory()
    {
        // Arrange

        var dbContext = this.fixture.CreateDbContext();
        var categoryExample = this.fixture.GetExampleCategory();
        await dbContext.AddRangeAsync(this.fixture.GetExampleCategoriesList(10));
        var tracking = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        tracking.State = EntityState.Detached;
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);

        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());


        var input = new DeleteCategoryInput(categoryExample.Id);
        var useCase = new DeleteCategory(repository, unitOfWork);

        // Act
        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = this.fixture.CreateDbContext(true);
        var category = await assertDbContext.Categories.FindAsync(categoryExample.Id);

        // Assert
        category.Should().BeNull();
        var assertCategories = await assertDbContext.Categories.CountAsync();
        assertCategories.Should().Be(10);
    }

    [Fact(DisplayName = nameof(DeleteCategoryThrowWhenCategoryNotFound))]
    [Trait("Infrastructure/Application ", "Delete Category - Use Case")]
    public async Task DeleteCategoryThrowWhenCategoryNotFound()
    {
        // Arrange


        var dbContext = this.fixture.CreateDbContext();
        var categoryExample = this.fixture.GetExampleCategory();
        await dbContext.AddRangeAsync(this.fixture.GetExampleCategoriesList(10));
        await dbContext.SaveChangesAsync();

        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());


        var input = new DeleteCategoryInput(categoryExample.Id);
        var useCase = new DeleteCategory(repository, unitOfWork);

        // Act
        var task = async() => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await task.Should().ThrowAsync<NotFoundException>();

    }




}
