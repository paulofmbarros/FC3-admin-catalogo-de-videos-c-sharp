// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.ListGenres;

using Extensions.DateTime;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.ListGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Infra.Data.EF.Models;
using Models;
using Xunit;

[Collection(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTests : IDisposable
{
    private readonly ListGenresApiTestFixture fixture;

    public ListGenresApiTests(ListGenresApiTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task ListGenres()
    {
        // Arrange
        var exampleGenres = fixture.GetExampleGenresList(10);
        await fixture.Persistence.InsertList(exampleGenres);
        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = exampleGenres.Count;
        // Act
        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        // Assert
        response.Should().NotBeNull();
        response.EnsureSuccessStatusCode();
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(exampleGenres.Count);

        output.Data.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
        });
    }

    [Fact(DisplayName = nameof(ListGenresWithRelations))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task ListGenresWithRelations()
    {
        // Arrange
        var exampleGenres = fixture.GetExampleGenresList(15);
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

        var input = new ListGenresInput();
        input.Page = 1;
        input.PerPage = exampleGenres.Count;

        // Act
        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        // Assert
        response.Should().NotBeNull();
        response.EnsureSuccessStatusCode();
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(exampleGenres.Count);

        output.Data.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
            var relatedCategoriesIds = outputItem.Categories.Select(c => c.Id).ToList();
            relatedCategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
            outputItem.Categories.ToList().ForEach(outputRelatedCategory =>
            {
                var exampleRelatedCategory = exampleCategories.Find(x => x.Id == outputRelatedCategory.Id);
                exampleRelatedCategory.Should().NotBeNull();
                outputRelatedCategory.Name.Should().Be(exampleRelatedCategory.Name);
            });
        });
    }

    [Fact(DisplayName = nameof(EmptyWhenAreNoItems))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task EmptyWhenAreNoItems()
    {
        // Arrange
        var input = new ListGenresInput
        {
            Page = 1,
            PerPage = 10
        };
        // Act
        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        // Assert
        response.Should().NotBeNull();
        response.EnsureSuccessStatusCode();
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(0);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task ListPaginated(int quantityGenresToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        // Arrange
        var exampleGenres = fixture.GetExampleGenresList(quantityGenresToGenerate);
        await fixture.Persistence.InsertList(exampleGenres);
        var input = new ListGenresInput();
        input.Page = page;
        input.PerPage = perPage;
        // Act
        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        // Assert
        response.Should().NotBeNull();
        response.EnsureSuccessStatusCode();
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(quantityGenresToGenerate);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedQuantityItems);

        output.Data.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
        });
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [InlineData("Action",1,5,1,1)]
    [InlineData("Horror",1,5,3,3)]
    [InlineData("Horror",2,5,0,3)]
    [InlineData("Sci-Fi",1,5,4,4)]
    [InlineData("Sci-Fi",1,2,2,4)]
    [InlineData("Sci-Fi",2,3,1,4)]
    [InlineData("Sci-Fi other",1,3,0,0)]
    [InlineData("Robots",1,5,2,2)]
    public async Task SearchByText(string search ,int page, int perPage, int expectedQuantityItemsReturned, int expectedQuantityTotalItems)
    {
        // Arrange
        var exampleGenres = fixture.GetExampleCategoriesListWithNames(
        [
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-Fi IA",
            "Sci-Fi Space",
            "Sci-Fi Robots",
            "Sci-Fi Future",
        ]);
        await fixture.Persistence.InsertList(exampleGenres);
        var input = new ListGenresInput
        {
            Page = page,
            PerPage = perPage,
            Search = search
        };

        // Act
        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        // Assert
        response.Should().NotBeNull();
        response.EnsureSuccessStatusCode();
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);

        output.Data.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
        });
    }

    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [InlineData("name","asc")]
    [InlineData("name","desc")]
    [InlineData("id","desc")]
    [InlineData("id","asc")]
    [InlineData("createdAt","asc")]
    [InlineData("createdAt","desc")]
    public async Task ListOrdered(string orderBy, string order)
    {
        // Arrange
        var exampleGenres = fixture.GetExampleGenresList(10);
        await fixture.Persistence.InsertList(exampleGenres);
        var input = new ListGenresInput
        {
            Page = 1,
            PerPage = 10,
            Direction = order switch
            {
                "asc" => SearchOrder.Asc,
                "desc" => SearchOrder.Desc,
                _ => throw new ArgumentOutOfRangeException(nameof(order))
            },
            Sort = orderBy
        };

        // Act
        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<GenreModelOutput>>("/genres", input);

        // Assert
        response.Should().NotBeNull();
        response.EnsureSuccessStatusCode();
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Total.Should().Be(10);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Data.Should().HaveCount(10);

        var expectedOrderedList = this.fixture.CloneGenresListOrdered(exampleGenres, orderBy, input.Direction );

        output.Should().NotBeNull();
        output.Meta.Total.Should().Be(expectedOrderedList.Count);
        output.Data.Should().HaveCount(expectedOrderedList.Count);
        output.Data.ToList().ForEach(outputItem =>
        {
            var exampleItem = expectedOrderedList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
        });
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
    }

    public void Dispose() => this.fixture.ClearPersistence();
}