// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.GetCategory;

using Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Xunit;

[Collection(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiTest
{
    private readonly GetCategoryApiTestFixture fixture;

    public GetCategoryApiTest(GetCategoryApiTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = "")]
    [Trait("EndToEnd/Api", "Category/GetCategory - Endpoints")]
    public async Task GetCategory()
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = exampleCategories[10];

        //act
        var (response, output) = await this.fixture.ApiClient.Get<CategoryModelOutput>($"/categories/{ exampleCategory.Id }");

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);

    }


}