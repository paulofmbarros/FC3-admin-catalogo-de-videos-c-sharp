// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.DeleteGenre;

using System.Net;
using Fc.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;
using Infra.Data.EF.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(DeleteGenreTestApiFixture))]
public class DeleteGenreTestApi
{
    private readonly DeleteGenreTestApiFixture fixture;

    public DeleteGenreTestApi(DeleteGenreTestApiFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("EndToEnd/API", "Genre/DeleteGenre - Endpoints")]
    public async Task DeleteGenre()
    {
        // Arrange
        var exampleGenres = this.fixture.GetExampleGenresList();
        var targetGenre = exampleGenres[5];
        await this.fixture.Persistence.InsertList(exampleGenres);

        // Act
        var (response, output) = await this.fixture.ApiClient.Delete<Genre>($"/genres/{targetGenre.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();

        var genre = await this.fixture.Persistence.GetById(targetGenre.Id);
        genre.Should().BeNull();

    }

    [Fact(DisplayName = nameof(WhenNotFound404))]
    [Trait("EndToEnd/API", "Genre/DeleteGenre - Endpoints")]
    public async Task WhenNotFound404()
    {
        // Arrange
        var exampleGenres = this.fixture.GetExampleGenresList();
        var randomGuid = Guid.NewGuid();
        await this.fixture.Persistence.InsertList(exampleGenres);

        // Act
        var (response, output) = await this.fixture.ApiClient.Delete<ProblemDetails>($"/genres/{randomGuid}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Detail.Should().Be($"Genre {randomGuid} not found");
        output.Type.Should().Be("NotFound");


    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/DeleteGenre - Endpoints")]
    public async Task DeleteGenreWithRelations()
    {
        // Arrange
        var exampleGenres = fixture.GetExampleGenresList();
        var targetGenre = exampleGenres[5];
        var exampleCategories = fixture.GetExampleCategoriesList();
        var random = new Random();

        foreach (var genre in exampleGenres)
        {
            var relationsCount = random.Next(2, exampleCategories.Count - 1);
            for (var i = 0; i < relationsCount; i++)
            {
                var category = exampleCategories[random.Next(0, exampleCategories.Count)];
                if (!genre.Categories.Contains(category.Id))
                {
                    genre.AddCategory(category.Id);
                }
            }
        }

        var genreCategories = new List<GenresCategories>();
        exampleGenres.ForEach(genre =>
            genre.Categories.ToList()
                .ForEach(categoryId => genreCategories.Add(new GenresCategories(categoryId, genre.Id))));

        await this.fixture.Persistence.InsertList(exampleGenres);
        await this.fixture.CategoryPersistence.InsertList(exampleCategories);
        await this.fixture.Persistence.InsertGenresCategoriesRelationsList(genreCategories);

        // Act
        var (response, output) = await this.fixture.ApiClient.Delete<Genre>($"/genres/{targetGenre.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();

        var genreDb = await this.fixture.Persistence.GetById(targetGenre.Id);
        genreDb.Should().BeNull();

        var relations = await this.fixture.Persistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        relations.Should().NotContain(x => x.GenreId == targetGenre.Id);

    }
    
}