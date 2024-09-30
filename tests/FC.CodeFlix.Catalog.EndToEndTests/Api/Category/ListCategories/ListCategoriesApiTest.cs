// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.ListCategories;

using Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
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
        output.Items.Should().HaveCount(defaultPerPage);
        output.Items.Should().BeEquivalentTo(exampleCategories.OrderBy(exampleCategories => exampleCategories.Name).Take(defaultPerPage));
    }

    public void Dispose() => this.fixture.ClearPersistence();

}