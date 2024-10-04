// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

using System.Net;
using Extensions.DateTime;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTest : IDisposable
{
    private readonly UpdateCategoryApiTestFixture fixture;

    public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("EndToEnd/Api", "Category/UpdateCategory - Endpoints")]
    public async Task UpdateCategory()
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = exampleCategories[10];

        var categoryInput = fixture.GetExampleInput(exampleCategory.Id);

        var (response, output) = await this.fixture.ApiClient.Put<CategoryModelOutput>(
            $"/categories/{ exampleCategory.Id }",
            categoryInput
        );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(categoryInput.Name);
        output.Description.Should().Be(categoryInput.Description);
        output.IsActive.Should().Be((bool)categoryInput.IsActive);
        output.CreatedAt.TrimMilliSeconds().Should().Be(exampleCategory.CreatedAt.TrimMilliSeconds());

    }

    [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("EndToEnd/Api", "Category/UpdateCategory - Endpoints")]
    public async Task UpdateCategoryOnlyName()
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = exampleCategories[10];

        var categoryInput = new UpdateCategoryInput(exampleCategory.Id, this.fixture.GetValidCategoryName());

        var (response, output) = await this.fixture.ApiClient.Put<CategoryModelOutput>(
            $"/categories/{ exampleCategory.Id }",
            categoryInput
        );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(categoryInput.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.TrimMilliSeconds().Should().Be(exampleCategory.CreatedAt.TrimMilliSeconds());

    }

    [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
    [Trait("EndToEnd/Api", "Category/UpdateCategory - Endpoints")]
    public async Task UpdateCategoryNameAndDescription()
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = exampleCategories[10];

        var categoryInput = new UpdateCategoryInput(
            exampleCategory.Id,
            this.fixture.GetValidCategoryName(),
            this.fixture.GetValidCategoryDescription());

        var (response, output) = await this.fixture.ApiClient.Put<CategoryModelOutput>(
            $"/categories/{ exampleCategory.Id }",
            categoryInput
        );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(categoryInput.Name);
        output.Description.Should().Be(categoryInput.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.TrimMilliSeconds().Should().Be(exampleCategory.CreatedAt.TrimMilliSeconds());

    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/Api", "Category/UpdateCategory - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var exampleCategory = Guid.NewGuid();

        var categoryInput = this.fixture.GetExampleInput(exampleCategory);

        var (response, output) = await this.fixture.ApiClient.Put<ProblemDetails>(
            $"/categories/{ exampleCategory }",
            categoryInput
        );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Category {exampleCategory} not found.");
        output.Type.Should().Be("NotFound");

    }

    [Theory(DisplayName = nameof(ErrorWhenInvalidInput))]
    [Trait("EndToEnd/Api", "Category/UpdateCategory - Endpoints")]
    [MemberData(nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs), MemberType = typeof(UpdateCategoryApiTestDataGenerator))]
    public async Task ErrorWhenInvalidInput(UpdateCategoryInput input, string errorMessage)
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(20);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);

        var categoryInput = exampleCategories[10];

        input = input with { Id = categoryInput.Id };

        var (response, output) = await this.fixture.ApiClient.Put<ProblemDetails>(
            $"/categories/{ input.Id }",
            input
        );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Title.Should().Be("One or more validation errors occurred.");
        output.Detail.Should().Be(errorMessage);
        output.Type.Should().Be("UnprocessableEntity");


    }

    public void Dispose() => this.fixture.ClearPersistence();


}