// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories;

using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
    [Trait("Integration/Application", "ListCategories - UseCase")]
    public async Task SearchReturnsListAndTotal()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleCategoryList = fixture.GetExampleCategoriesList(15);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(1,20);

        var useCase = new ListCategories(categoryRepository);
        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);
        output.Items.Should().BeEquivalentTo(exampleCategoryList, options => options.Excluding(o => o.Events));
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    [Trait("Integration/Application", "ListCategories - UseCase")]
    public async Task SearchReturnsEmptyWhenEmpty()
    {
        var dbContext = this.fixture.CreateDbContext(false);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(1,20);

        var useCase = new ListCategories(categoryRepository);
        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);

    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Application", "ListCategories - UseCase")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task SearchReturnsPaginated(int quantityCategoriesToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCategoryList = this.fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(page,perPage);


        var useCase = new ListCategories(categoryRepository);
        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(quantityCategoriesToGenerate);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application",  "ListCategories - UseCase")]
    [InlineData("Action",1,5,1,1)]
    [InlineData("Horror",1,5,3,3)]
    [InlineData("Horror",2,5,0,3)]
    [InlineData("Sci-Fi",1,5,4,4)]
    [InlineData("Sci-Fi",1,2,2,4)]
    [InlineData("Sci-Fi",2,3,1,4)]
    [InlineData("Sci-Fi other",1,3,0,0)]
    [InlineData("Robots",1,5,2,2)]
    public async Task SearchByText(string search ,int page, int perPage, int expectedQuantityItemsReturned, int expectedQuantityTotalItems)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCategoryList = this.fixture.GetExampleCategoriesListWithNames(
        [
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-Fi IA",
            "Sci-Fi Space",
            "Sci-Fi Robots",
            "Sci-Fi Future",
        ]);

        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(page,perPage, search,"", SearchOrder.Asc);


        var useCase = new ListCategories(categoryRepository);
        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Application",  "ListCategories - UseCase")]
    [InlineData("name","asc")]
    [InlineData("name","desc")]
    [InlineData("id","desc")]
    [InlineData("id","asc")]
    [InlineData("createdAt","asc")]
    [InlineData("createdAt","desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCategoryList = this.fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchOrder = order.Equals("asc", StringComparison.CurrentCultureIgnoreCase) ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new ListCategoriesInput(1,20, "",orderBy, searchOrder);


        var useCase = new ListCategories(categoryRepository);
        var output = await useCase.Handle(searchInput, CancellationToken.None);
        var expectedOrderedList = this.fixture.CloneCategoriesListOrdered(exampleCategoryList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);
        output.Items.Should().BeEquivalentTo(expectedOrderedList, options =>
        {
            options.Excluding(x => x.Events);
            return options.WithStrictOrdering();
        });
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);


    }

}
