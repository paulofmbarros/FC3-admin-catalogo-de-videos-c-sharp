// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.CreateGenre;

using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;
using Xunit;

[Collection(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTest
{
    private readonly CreateGenreApiTestFixture fixture;

    public CreateGenreApiTest(CreateGenreApiTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    public async Task CreateGenre()
    {
        // Arrange
        var genre = this.fixture.GetExampleGenre();

        // Act
        var (response, output) = await this.fixture.ApiClient.Post<ApiResponse<GenreModelOutput>>("/genres", genre);

        // Assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(genre.Name);
        output.Data.IsActive.Should().Be(genre.IsActive);
        output.Data.Categories.Should().HaveCount(0);

        var genreFromDb = await this.fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(genre.Name);
        genreFromDb.IsActive.Should().Be(genre.IsActive);

    }

    [Fact(DisplayName = nameof(CreateGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    public async Task CreateGenreWithRelations()
    {
        // Arrange
        var exampleCategories = this.fixture.GetExampleCategoriesList(10);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);
        var relatedCategories = exampleCategories
            .Skip(3)
            .Take(3)
            .Select(x=>x.Id)
            .ToList();


        var apiInput = new CreateGenreInput(fixture.GetValidCategoryName(), this.fixture.GetRandomBoolean(),
            relatedCategories);

        // Act
        var (response, output) = await this.fixture.ApiClient.Post<ApiResponse<GenreModelOutput>>("/genres", apiInput);

        // Assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(apiInput.Name);
        output.Data.IsActive.Should().Be(apiInput.IsActive);
        output.Data.Categories.Should().HaveCount(relatedCategories.Count);

        var outputRelatedCategoryIds = output.Data.Categories.Select(x => x.Id).ToList();
        outputRelatedCategoryIds.Should().BeEquivalentTo(relatedCategories);

        var genreFromDb = await this.fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(apiInput.Name);
        genreFromDb.IsActive.Should().Be(apiInput.IsActive);
        var relationsFromDb = await this.fixture.Persistence.GetGenresCategoriesRelationsByGenreId(genreFromDb.Id);
        relationsFromDb.Should().NotBeNull();
        relationsFromDb.Should().HaveCount(relatedCategories.Count);
        relationsFromDb.Select(x => x.CategoryId).Should().BeEquivalentTo(relatedCategories);
        var relatedCategoriesIdsFromDb = relationsFromDb.Select(x => x.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(relatedCategories);




    }
    
}