// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.GetGenre;

using System.Net;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Infra.Data.EF.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest(GetGenreApiTestFixture fixture)
{
    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    public async Task GetGenre()
    {
        var exampleGenres = fixture.GetExampleGenresList();
        var targetGenre = exampleGenres[5];
        await fixture.Persistence.InsertList(exampleGenres);

        //act

        var (response, output) =
            await fixture.ApiClient.Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);

    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    public async Task NotFound()
    {
        var exampleGenres = fixture.GetExampleGenresList();
        var targetGenre = Guid.NewGuid();
        await fixture.Persistence.InsertList(exampleGenres);

        //act

        var (response, output) =
            await fixture.ApiClient.Get<ProblemDetails>($"/genres/{targetGenre}");

        //assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre {targetGenre} not found");

    }

    [Fact(DisplayName = nameof(GetGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    public async Task GetGenreWithRelations()
    {
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

        await fixture.Persistence.InsertList(exampleGenres);
        await fixture.CategoryPersistence.InsertList(exampleCategories);
        await fixture.Persistence.InsertGenresCategoriesRelationsList(genreCategories);

        //act

        var (response, output) =
            await fixture.ApiClient.Get<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}");

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
        var relatedCategoriesIds = output.Data.Categories.Select(c => c.Id).ToList();
        relatedCategoriesIds.Should().BeEquivalentTo(targetGenre.Categories);

    }

}