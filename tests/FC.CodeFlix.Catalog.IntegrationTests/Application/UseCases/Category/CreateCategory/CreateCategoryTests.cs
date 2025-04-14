// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTests
{
    private readonly CreateCategoryTestFixture fixture;

    public CreateCategoryTests(CreateCategoryTestFixture fixture) => this.fixture = fixture;


    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Integration/Application", "CreateCategory - Use Case ")]
    public async void CreateCategory()
    {
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());

        var useCase = new CreateCategory(unitOfWork, repository);

        var input = this.fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await this.fixture.CreateDbContext(true).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        dbCategory.IsActive.Should().Be(output.IsActive);

    }


    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Integration/Application", "CreateCategory - Use Case ")]
    public async void CreateCategoryWithOnlyName()
    {
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());

        var useCase = new CreateCategory(unitOfWork, repository);

        var input = new CreateCategoryInput(this.fixture.GetValidCategoryName(), string.Empty, null);

        var output = await useCase.Handle(input, CancellationToken.None);
        var dbCategory = await this.fixture.CreateDbContext(true).Categories.FindAsync(output.Id);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        dbCategory.Name.Should().Be(input.Name);
        output.Description.Should().Be(string.Empty);
        dbCategory.Description.Should().Be(string.Empty);
        output.IsActive.Should().BeTrue();
        dbCategory.IsActive.Should().BeTrue();
        output.CreatedAt.Should().NotBe(default);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

    }

    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    [Trait("Integration/Application ", "CreateCategory - Use Case ")]
    public async Task CreateCategoryWithOnlyNameAndDescription()
    {
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());

        var useCase = new CreateCategory(unitOfWork, repository);

        var input = new CreateCategoryInput(this.fixture.GetValidCategoryName(), this.fixture.GetValidCategoryDescription(), true);

        var output = await useCase.Handle(input, CancellationToken.None);
        var dbCategory = await this.fixture.CreateDbContext(true).Categories.FindAsync(output.Id);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.CreatedAt.Should().NotBe(default);
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().BeTrue();
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [Trait("Integration/Application ", "CreateCategory - Use Case ")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 6,
        MemberType = typeof(CreateCategoryTestDataGenerator))]
    public async void ThrowWhenCantInstantiateCategory(CreateCategoryInput input, string exceptionMessage)
    {
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(dbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());

        var useCase = new CreateCategory(unitOfWork, repository);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);

    }
}
