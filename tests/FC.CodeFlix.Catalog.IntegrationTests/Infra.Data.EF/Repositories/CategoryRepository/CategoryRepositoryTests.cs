// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTests
{
    private readonly CategoryRepositoryTestFixture fixture;

    public CategoryRepositoryTests(CategoryRepositoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();

        var categoryRepository = new CategoryRepository(dbContext);

        await categoryRepository.Insert(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();
        var exampleCategoryList = fixture.GetExampleCategoriesList();
        exampleCategoryList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);

        var result = await categoryRepository.Get(exampleCategory.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(exampleCategory.Name);
        result.Description.Should().Be(exampleCategory.Description);
        result.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        result.IsActive.Should().Be(exampleCategory.IsActive);
        result.Id.Should().Be(exampleCategory.Id);


    }

    [Fact(DisplayName = nameof(GetThrowsIfNotFound))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task GetThrowsIfNotFound()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        await dbContext.AddRangeAsync(fixture.GetExampleCategoriesList());
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);

        var task = async () => await categoryRepository.Get(exampleId, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{exampleId}' not found");

    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();
        var newCategoryValues = fixture.GetExampleCategory();
        var exampleCategoryList = fixture.GetExampleCategoriesList();
        exampleCategoryList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        exampleCategory.Update(newCategoryValues.Name, newCategoryValues.Description);

        await categoryRepository.Update(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await dbContext.Categories.FindAsync(exampleCategory.Id);

        result.Should().NotBeNull();
        result.Name.Should().Be(exampleCategory.Name);
        result.Description.Should().Be(exampleCategory.Description);
        result.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        result.IsActive.Should().Be(exampleCategory.IsActive);
        result.Id.Should().Be(exampleCategory.Id);


    }

}