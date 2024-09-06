// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
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

        var dbCategory = await this.fixture.CreateDbContext(true).Categories.FindAsync(exampleCategory.Id);
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
        var categoryRepository = new CategoryRepository(this.fixture.CreateDbContext(true));

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

        var result = await this.fixture
            .CreateDbContext(true).Categories
            .FindAsync(exampleCategory.Id);

        result.Should().NotBeNull();
        result.Name.Should().Be(exampleCategory.Name);
        result.Description.Should().Be(exampleCategory.Description);
        result.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        result.IsActive.Should().Be(exampleCategory.IsActive);
        result.Id.Should().Be(exampleCategory.Id);


    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();
        var exampleCategoryList = fixture.GetExampleCategoriesList();
        exampleCategoryList.Add(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);

        await categoryRepository.Delete(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await this.fixture
            .CreateDbContext(true).Categories
            .FindAsync(exampleCategory.Id);

        result.Should().BeNull();

    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearchReturnsListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoriesList(15);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new SearchInput(1,20,"","", SearchOrder.Asc);


       var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);
        output.Items.Should().BeEquivalentTo(exampleCategoryList);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        CodeflixCatalogDbContext dbContext = this.fixture.CreateDbContext();
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new SearchInput(1,20,"","", SearchOrder.Asc);

        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task SearchReturnsPaginated(int quantityCategoriesToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        CodeflixCatalogDbContext dbContext = this.fixture.CreateDbContext();
        var exampleCategoryList = this.fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new SearchInput(page,perPage,"","", SearchOrder.Asc);


        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(quantityCategoriesToGenerate);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.Should().BeEquivalentTo(exampleCategoryList.Skip((page-1)*perPage).Take(perPage));
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }


}