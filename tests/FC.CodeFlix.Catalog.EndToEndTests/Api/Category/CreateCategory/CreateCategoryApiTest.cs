// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.CreateCategory;

using System.Net;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest
{
    private readonly CreateCategoryApiTestFixture fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    [Trait("EndtoEnd/Api", "Category - Endpoints")]
    public async Task CreateCategory()
    {
        // Arrange
        var input = this.fixture.GetExampleInput();

        // Act
        var (response, output) = await this.fixture.ApiClient.Post<CategoryModelOutput>("/categories", input);

        // Assert
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.CreatedAt.Should().NotBe(default);
        var dbCategory = await this.fixture.CategoryPersistence.GetById(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive);



    }

}