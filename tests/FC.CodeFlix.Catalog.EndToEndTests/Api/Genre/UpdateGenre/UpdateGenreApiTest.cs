// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.UpdateGenre;

using System.Net;
using Fc.CodeFlix.Catalog.Api.ApiModels.Genre;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using Infra.Data.EF.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTest
{

    private readonly UpdateGenreApiTestFixture fixture;

    public UpdateGenreApiTest(UpdateGenreApiTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task UpdateGenre()
    {
        var exampleGenres = fixture.GetExampleGenresList();
        var targetGenre = exampleGenres[5];
        await fixture.Persistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(fixture.GetGenreName(), fixture.GetRandomBoolean());
        //act

        var (response, output) =
            await fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        var genreFromDb = await this.fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
    }

    [Fact(DisplayName = nameof(ProblemDetailsWhenNotFound))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task ProblemDetailsWhenNotFound()
    {
        var exampleGenres = fixture.GetExampleGenresList();
        var randomGuid = Guid.NewGuid();
        await fixture.Persistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(fixture.GetGenreName(), fixture.GetRandomBoolean());
        //act

        var (response, output) =
            await fixture.ApiClient.Put<ProblemDetails>($"/genres/{randomGuid}", input);

        //assert
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Detail.Should().Be($"Genre {randomGuid} not found");
        output.Type.Should().Be("NotFound");
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Genre {randomGuid} not found");

    }

    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task UpdateGenreWithRelations()
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

        var newRelationsCount = random.Next(2, exampleCategories.Count - 1);
        var newRelationsCategoriesIds = new List<Guid>();
        for (var i = 0; i < newRelationsCount; i++)
        {
            var selectedCategoryIndex = random.Next(0, exampleCategories.Count - 1);
            var selected = exampleCategories[selectedCategoryIndex];
            var category = exampleCategories[random.Next(0, exampleCategories.Count)];
            if (!newRelationsCategoriesIds.Contains(selected.Id))
            {
                newRelationsCategoriesIds.Add(selected.Id);
            }
        }


        await fixture.Persistence.InsertList(exampleGenres);
        await fixture.CategoryPersistence.InsertList(exampleCategories);
        await fixture.Persistence.InsertGenresCategoriesRelationsList(genreCategories);
        var input = new UpdateGenreApiInput(fixture.GetGenreName(), fixture.GetRandomBoolean(), newRelationsCategoriesIds);

        //act
        var (response, output) =
            await fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        var genreFromDb = await this.fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
        var relatedCategoriesIdsFromOutput = output.Data.Categories.Select(c => c.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(newRelationsCategoriesIds);
        var genresCategoriesFromDb = await this.fixture.Persistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        var relatedCategoriesIdsFromDb = genresCategoriesFromDb.Select(gc => gc.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(newRelationsCategoriesIds);

    }

    [Fact(DisplayName = nameof(ErrorWhenInvalidRelation))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task ErrorWhenInvalidRelation()
    {
        var exampleGenres = fixture.GetExampleGenresList();
        var targetGenre = exampleGenres[5];
        var randomGuid = Guid.NewGuid();
        await fixture.Persistence.InsertList(exampleGenres);
        var input = new UpdateGenreApiInput(fixture.GetGenreName(), fixture.GetRandomBoolean(), new List<Guid>{randomGuid});
        //act

        var (response, output) =
            await fixture.ApiClient.Put<ProblemDetails>($"/genres/{targetGenre.Id}", input);

        //assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category Id (or ids) not found: {randomGuid}");

    }

     [Fact(DisplayName = nameof(PersistsRelationsWhenNotPresentInInput))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task PersistsRelationsWhenNotPresentInInput()
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
        var input = new UpdateGenreApiInput(fixture.GetGenreName(), fixture.GetRandomBoolean());

        //act
        var (response, output) =
            await fixture.ApiClient.Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        //assert
        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output!.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.IsActive!);
        var genreFromDb = await this.fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be((bool)input.IsActive);
        var relatedCategoriesIdsFromOutput = output.Data.Categories.Select(c => c.Id).ToList();
        relatedCategoriesIdsFromOutput.Should().BeEquivalentTo(targetGenre.Categories);
        var genresCategoriesFromDb = await this.fixture.Persistence.GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        var relatedCategoriesIdsFromDb = genresCategoriesFromDb.Select(gc => gc.CategoryId).ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(targetGenre.Categories);

    }
}