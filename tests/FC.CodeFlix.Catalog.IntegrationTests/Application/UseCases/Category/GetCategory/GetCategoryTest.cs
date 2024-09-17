// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory;

using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FluentAssertions;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Integration/Application ", "GetCategory - Use Cases")]
    public async Task GetCategory()
    {
        // Arrange
        var dbContext = this.fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var exampleCategory = this.fixture.GetExampleCategory();
        dbContext.Categories.Add(exampleCategory);
        await dbContext.SaveChangesAsync();

        var input = new GetCategoryInput(exampleCategory.Id);
        var useCase = new GetCategory(repository);
        // Act

        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesNotExist))]
    [Trait("Application ", "GetCategory - Use Cases")]
    public async Task NotFoundExceptionWhenCategoryDoesNotExist()
    {
        // Arrange
        var dbContext = this.fixture.CreateDbContext();
        var exampleCategory = this.fixture.GetExampleCategory();
        dbContext.Categories.Add(exampleCategory);
        await dbContext.SaveChangesAsync();

        var repository = new CategoryRepository(dbContext);

        var input = new GetCategoryInput(Guid.NewGuid());
        var useCase = new GetCategory(repository);
        // Act

        var task = async() => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found");

    }

}