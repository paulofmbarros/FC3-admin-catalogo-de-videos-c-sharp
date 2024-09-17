// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{

    private readonly UpdateCategoryTestFixture fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task UpdateCategory(Category exampleCategory, UpdateCategoryInput input )
    {
        // Arrange
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        await dbContext.Categories.AddRangeAsync(this.fixture.GetExampleCategory());
        var trackingInfo =  await dbContext.Categories.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;

        var useCase = new UpdateCategory(repository, unitOfWork);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        var updatedCategory = await this.fixture.CreateDbContext(true).Categories.FindAsync(output.Id);

        // Assert
        Assert.NotNull(output);
        output.Should().BeOfType<CategoryModelOutput>();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Id.Should().Be(input.Id);
        updatedCategory.Name.Should().Be(input.Name);
        updatedCategory.Description.Should().Be(input.Description);
        updatedCategory.IsActive.Should().Be((bool)input.IsActive);

    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutProvidingIsActive))]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task UpdateCategoryWithoutProvidingIsActive(Category exampleCategory, UpdateCategoryInput exampleInput )
    {
        // Arrange
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        await dbContext.Categories.AddRangeAsync(this.fixture.GetExampleCategory());
        var trackingInfo =  await dbContext.Categories.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;

        var useCase = new UpdateCategory(repository, unitOfWork);
        var input = new UpdateCategoryInput(exampleCategory.Id, exampleCategory.Name, exampleCategory.Description);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        var updatedCategory = await this.fixture.CreateDbContext(true).Categories.FindAsync(output.Id);

        // Assert
        Assert.NotNull(output);
        output.Should().BeOfType<CategoryModelOutput>();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Id.Should().Be(input.Id);
        updatedCategory.Name.Should().Be(input.Name);
        updatedCategory.Description.Should().Be(input.Description);
        updatedCategory.IsActive.Should().Be(exampleCategory.IsActive);
    }

    [Theory(DisplayName = nameof(UpdateCategoryWithOnlyName))]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task UpdateCategoryWithOnlyName(Category exampleCategory, UpdateCategoryInput exampleInput )
    {
        // Arrange
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        await dbContext.Categories.AddRangeAsync(this.fixture.GetExampleCategory());
        var trackingInfo =  await dbContext.Categories.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;

        var input = new UpdateCategoryInput(exampleCategory.Id, exampleCategory.Name);

        var useCase = new UpdateCategory(repository, unitOfWork);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);
        var updatedCategory = await this.fixture.CreateDbContext(true).Categories.FindAsync(output.Id);


        // Assert
        Assert.NotNull(output);
        output.Should().BeOfType<CategoryModelOutput>();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Id.Should().Be(input.Id);
        updatedCategory.Name.Should().Be(input.Name);
        updatedCategory.Description.Should().Be(exampleCategory.Description);
        updatedCategory.IsActive.Should().Be(exampleCategory.IsActive);

    }

    [Fact(DisplayName = nameof(ThrowWhenNotFoundCategory))]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    public async Task ThrowWhenNotFoundCategory()
    {
        // Arrange
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        await dbContext.Categories.AddRangeAsync(this.fixture.GetExampleCategory());
        var exampleCategory = this.fixture.GetExampleCategory();
        var trackingInfo =  await dbContext.Categories.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;

        var input = this.fixture.GenerateUpdateCategoryInput();

        var useCase = new UpdateCategory(repository, unitOfWork);

        // Act
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found");

    }


    [Theory(DisplayName = nameof(UpdateThrowsWhenCantInstantiateCategory))]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task UpdateThrowsWhenCantInstantiateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
    {
        // Arrange
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var exampleCategory = this.fixture.GetExampleCategory();
        var trackingInfo =  await dbContext.Categories.AddAsync(exampleCategory);
        await dbContext.SaveChangesAsync();
        input = input with { Id = exampleCategory.Id };
        trackingInfo.State = EntityState.Detached;

        var useCase = new UpdateCategory(repository, unitOfWork);

        // Act
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);


    }


}