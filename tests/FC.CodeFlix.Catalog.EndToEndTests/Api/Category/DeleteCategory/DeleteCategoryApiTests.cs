// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.DeleteCategory;

using System.Net;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(DeleteCategoryApiTestFixture))]
public class DeleteCategoryApiTests
{
    private readonly DeleteCategoryApiTestFixture fixture;

    public DeleteCategoryApiTests(DeleteCategoryApiTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("EndToEnd/Api", "Category/DeleteCategory - Endpoints")]
    public async Task DeleteCategory()
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = exampleCategories[10];

        //act
        var (response, output) = await this.fixture.ApiClient.Delete<object>($"/categories/{ exampleCategory.Id }");

        //assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();

        var category = await this.fixture.CategoryPersistence.GetById(exampleCategory.Id);
        category.Should().BeNull();
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/Api", "Category/DeleteCategory - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        //arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = Guid.NewGuid();

        //act
        var (response, output) = await this.fixture.ApiClient.Delete<ProblemDetails>($"/categories/{ exampleCategory }");

        //assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        output.Should().NotBeNull();
        output.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Detail.Should().Be($"Category {exampleCategory} not found.");




    }
}