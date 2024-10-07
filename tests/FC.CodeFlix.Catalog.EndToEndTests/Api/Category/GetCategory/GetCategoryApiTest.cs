// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.GetCategory;

using System.Net;
using Extensions.DateTime;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiTest : IDisposable
{
    private readonly GetCategoryApiTestFixture fixture;

    public GetCategoryApiTest(GetCategoryApiTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("EndToEnd/Api", "Category/GetCategory - Endpoints")]
    public async Task GetCategory()
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = exampleCategories[10];

        //act
        var (response, output) =
            await this.fixture.ApiClient.Get<ApiResponse<CategoryModelOutput>>($"/categories/{exampleCategory.Id}");

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(exampleCategory.Name);
        output.Data.Description.Should().Be(exampleCategory.Description);
        output.Data.IsActive.Should().Be(exampleCategory.IsActive);
        output.Data.CreatedAt.TrimMilliSeconds().Should().Be(exampleCategory.CreatedAt.TrimMilliSeconds());
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/Api", "Category/GetCategory - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = Guid.NewGuid();

        //act
        var (response, output) = await this.fixture.ApiClient.Get<ProblemDetails>($"/categories/{exampleCategory}");

        //assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category {exampleCategory} not found.");
        output.Type.Should().Be("NotFound");
    }

    public void Dispose() => this.fixture.ClearPersistence();
}