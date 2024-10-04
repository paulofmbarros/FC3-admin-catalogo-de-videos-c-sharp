// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.ListCategories;

using Extensions.DateTime;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Xunit;

[Collection(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTest : IDisposable
{
    private readonly ListCategoriesApiTestFixture fixture;

    public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    [Trait("EndToEnd/API ", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalByDefault()
    {
        //arrange
        var defaultPerPage = 15;
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        //act
        var (response, output) = await this.fixture.ApiClient.Get<ListCategoriesOutput>($"/categories");


        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategories.Count);
        output.Page.Should().Be(1);
        output.PerPage.Should().Be(defaultPerPage);
        output.Items.Should().HaveCount(defaultPerPage);
        foreach (CategoryModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleCategories
                .FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(
                exampleItem.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API ", "Category/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        //act
        var (response, output) = await this.fixture.ApiClient.Get<ListCategoriesOutput>($"/categories");

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
        output.Page.Should().Be(1);
        output.PerPage.Should().Be(15);
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API ", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var input = new ListCategoriesInput(1, 5);

        //act
        var (response, output) = await this.fixture.ApiClient.Get<ListCategoriesOutput>($"/categories", input);


        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategories.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Items.Should().HaveCount(input.PerPage);

        foreach (CategoryModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleCategories
                .FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(
                exampleItem.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API ", "Category/List - Endpoints")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task ListPaginated(int quantityCategoriesToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var input = new ListCategoriesInput(page, perPage);

        //act
        var (response, output) = await this.fixture.ApiClient.Get<ListCategoriesOutput>($"/categories", input);


        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategories.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Items.Should().HaveCount(expectedQuantityItems);

        foreach (CategoryModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleCategories
                .FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(
                exampleItem.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API ", "Category/List - Endpoints")]
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
        //arrange
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

        await this.fixture.CategoryPersistence.InsertList(exampleCategoryList);

        var input = new ListCategoriesInput(page, perPage, search);

        //act
        var (response, output) = await this.fixture.ApiClient.Get<ListCategoriesOutput>($"/categories", input);


        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
    }

    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("EndToEnd/API ", "Category/List - Endpoints")]
    [InlineData("name","asc")]
    [InlineData("name","desc")]
    [InlineData("id","desc")]
    [InlineData("id","asc")]
    public async Task ListOrdered(string orderBy, string order)
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(10);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);
        var searchOrder = order.Equals("asc", StringComparison.CurrentCultureIgnoreCase) ? SearchOrder.Asc : SearchOrder.Desc;


        var input = new ListCategoriesInput(1, 20 , "", orderBy, searchOrder);

        //act
        var (response, output) = await this.fixture.ApiClient.Get<ListCategoriesOutput>($"/categories", input);

        var expectedOrderedList = this.fixture.CloneCategoriesListOrdered(exampleCategories, orderBy, searchOrder);

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategories.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Should().NotBeNull();
        output.Total.Should().Be(expectedOrderedList.Count);
        output.Items.Should().HaveCount(expectedOrderedList.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        for(var indice = 0; indice< expectedOrderedList.Count; indice++)
        {
          var outputItem = output.Items[indice];
          var exampleItem = expectedOrderedList[indice];
          outputItem.Should().NotBeNull();
          exampleItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
        }
    }

     [Theory(DisplayName = nameof(ListOrderedDates))]
    [Trait("EndToEnd/API ", "Category/List - Endpoints")]
    [InlineData("createdAt","asc")]
    [InlineData("createdAt","desc")]
    public async Task ListOrderedDates(string orderBy, string order)
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(10);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);
        var searchOrder = order.Equals("asc", StringComparison.CurrentCultureIgnoreCase) ? SearchOrder.Asc : SearchOrder.Desc;


        var input = new ListCategoriesInput(1, 20 , "", orderBy, searchOrder);

        //act
        var (response, output) = await this.fixture.ApiClient.Get<ListCategoriesOutput>($"/categories", input);


        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCategories.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Should().NotBeNull();
        output.PerPage.Should().Be(input.PerPage);
        DateTime? lastItemDate = null;

        foreach (CategoryModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleCategories.FirstOrDefault(c => c.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());

            if (lastItemDate is null)
            {
                continue;
            }

            if (searchOrder == SearchOrder.Asc)
            {
                outputItem.CreatedAt.Should().BeOnOrAfter(lastItemDate.Value);
            }
            else
            {
                outputItem.CreatedAt.Should().BeOnOrBefore(lastItemDate.Value);
            }

            lastItemDate = outputItem.CreatedAt;

        }
    }

    public void Dispose() => this.fixture.ClearPersistence();

}