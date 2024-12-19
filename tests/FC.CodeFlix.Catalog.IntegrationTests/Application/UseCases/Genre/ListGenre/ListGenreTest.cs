// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenre;

using Catalog.Infra.Data.EF.Models;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.ListGenre;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;

[Collection(nameof(ListGenreTestFixture))]
public class ListGenreTest
{
    private readonly ListGenreTestFixture fixture;

    public ListGenreTest(ListGenreTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("Integration/Application ", "ListGenres - UseCases")]
    public async Task ListGenres()
    {
        var exampleGenre = this.fixture.GetExampleGenresList();
        var arrangeDbContext = this.fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenre);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);
        var useCase = new ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));

        var input = new ListGenresInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(exampleGenre.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenre.Count);
        output.Items.Should().BeEquivalentTo(exampleGenre);

    }

    [Fact(DisplayName = nameof(ListGenresReturnsWEmptyWhenPersisitenceIsEmpty))]
    [Trait("Integration/Application ", "ListGenres - UseCases")]
    public async Task ListGenresReturnsWEmptyWhenPersisitenceIsEmpty()
    {
        var arrangeDbContext = this.fixture.CreateDbContext();
        var useCase = new ListGenres(new GenreRepository(arrangeDbContext), new CategoryRepository(arrangeDbContext));

        var input = new ListGenresInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(0);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
    }

    [Fact(DisplayName = nameof(ListGenresVerifyRelations))]
    [Trait("Integration/Application ", "ListGenres - UseCases")]
    public async Task ListGenresVerifyRelations()
    {
        var exampleGenre = this.fixture.GetExampleGenresList();
        var exampleCategories = this.fixture.GetExampleCategoriesList();
        var random = new Random();

        foreach (var genre in exampleGenre)
        {
            var relationsCount = random.Next(1, 3);
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
        exampleGenre.ForEach(genre =>
            genre.Categories.ToList()
                .ForEach(categoryId => genreCategories.Add(new GenresCategories(categoryId, genre.Id))));


        var arrangeDbContext = this.fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenre);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genreCategories);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = this.fixture.CreateDbContext(true);


        var useCase = new ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));

        var input = new ListGenresInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(exampleGenre.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenre.Count);

        //Aqui iteramos sobre os genres, ou seja cada Item representa um genre e tem varias categorias associadas a ele
        output.Items.Select(x=>x.Id).Should().BeEquivalentTo(exampleGenre.Select(x=>x.Id));
        output.Items.Select(x=>x.Name).Should().BeEquivalentTo(exampleGenre.Select(x=>x.Name));

        // para cada categoria associada a um genre, verificamos se a categoria existe na lista de categorias de exemplo
        foreach (var item in output.Items.SelectMany(x=>x.Categories))
        {
            var exampleCategory = exampleCategories.Find(x => x.Id == item.Id);
            exampleCategory.Should().NotBeNull();
            item.Id.Should().Be(exampleCategory!.Id);
            item.Name.Should().Be(exampleCategory!.Name);

        }

    }

    [Theory(DisplayName = nameof(ListGenresPaginated))]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    [Trait("Integration/Application ", "ListGenres - UseCases")]
    public async Task ListGenresPaginated(int quantityToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        var exampleGenre = this.fixture.GetExampleGenresList(quantityToGenerate);
        var exampleCategories = this.fixture.GetExampleCategoriesList();
        var random = new Random();

        foreach (var genre in exampleGenre)
        {
            var relationsCount = random.Next(1, 3);
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
        exampleGenre.ForEach(genre =>
            genre.Categories.ToList()
                .ForEach(categoryId => genreCategories.Add(new GenresCategories(categoryId, genre.Id))));


        var arrangeDbContext = this.fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenre);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genreCategories);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = this.fixture.CreateDbContext(true);


        var useCase = new ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));

        var input = new ListGenresInput(page, perPage);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenre.Count);

        //Aqui iteramos sobre os genres, ou seja cada Item representa um genre e tem varias categorias associadas a ele
        if (expectedQuantityItems > 0)
        {
            exampleGenre.Select(x => x.Id).Should().Contain(output?.Items?.Select(x => x.Id));
            exampleGenre.Select(x => x.Name).Should().Contain(output?.Items?.Select(x => x.Name));
        }

        // para cada categoria associada a um genre, verificamos se a categoria existe na lista de categorias de exemplo
        foreach (var item in output.Items.SelectMany(x=>x.Categories))
        {
            var exampleCategory = exampleCategories.Find(x => x.Id == item.Id);
            exampleCategory.Should().NotBeNull();
            item.Id.Should().Be(exampleCategory!.Id);
            item.Name.Should().Be(exampleCategory!.Name);

        }

    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
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

        var exampleGenresList = this.fixture.GetExampleGenreListByNames([
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

        var exampleCategories = this.fixture.GetExampleCategoriesList();
        var random = new Random();

        foreach (var genre in exampleGenresList)
        {
            var relationsCount = random.Next(1, 3);
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
        exampleGenresList.ForEach(genre =>
            genre.Categories.ToList()
                .ForEach(categoryId => genreCategories.Add(new GenresCategories(categoryId, genre.Id))));


        var arrangeDbContext = this.fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenresList);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genreCategories);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = this.fixture.CreateDbContext(true);


        var useCase = new ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));

        var input = new ListGenresInput(page, perPage, search);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);

        //Aqui iteramos sobre os genres, ou seja cada Item representa um genre e tem varias categorias associadas a ele
        if (expectedQuantityItemsReturned > 0)
        {
            exampleGenresList.Select(x => x.Id).Should().Contain(output?.Items?.Select(x => x.Id));
            exampleGenresList.Select(x => x.Name).Should().Contain(output?.Items?.Select(x => x.Name));
        }

        // para cada categoria associada a um genre, verificamos se a categoria existe na lista de categorias de exemplo
        foreach (var item in output.Items.SelectMany(x=>x.Categories))
        {
            var exampleCategory = exampleCategories.Find(x => x.Id == item.Id);
            exampleCategory.Should().NotBeNull();
            item.Id.Should().Be(exampleCategory!.Id);
            item.Name.Should().Be(exampleCategory!.Name);

        }

    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData("name","asc")]
    [InlineData("name","desc")]
    [InlineData("id","desc")]
    [InlineData("id","asc")]
    [InlineData("createdAt","asc")]
    [InlineData("createdAt","desc")]
    public async Task SearchOrdered(string orderBy, string order){

        var exampleGenreList = this.fixture.GetExampleGenresList(10);
        var exampleCategories = this.fixture.GetExampleCategoriesList();
        var random = new Random();

        foreach (var genre in exampleGenreList)
        {
            var relationsCount = random.Next(1, 3);
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
        exampleGenreList.ForEach(genre =>
            genre.Categories.ToList()
                .ForEach(categoryId => genreCategories.Add(new GenresCategories(categoryId, genre.Id))));


        var arrangeDbContext = this.fixture.CreateDbContext();
        await arrangeDbContext.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.AddRangeAsync(exampleCategories);
        await arrangeDbContext.AddRangeAsync(genreCategories);
        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = this.fixture.CreateDbContext(true);


        var useCase = new ListGenres(new GenreRepository(actDbContext), new CategoryRepository(actDbContext));

        var orderEnum = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;

        var input = new ListGenresInput(1, 20, sort: orderBy, direction: orderEnum);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(exampleGenreList.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenreList.Count);
        var expectedOrderedList = this.fixture.CloneGenresListOrdered(exampleGenreList, orderBy, orderEnum);


        output.Should().NotBeNull();
        output.Total.Should().Be(exampleGenreList.Count);
        output.Items.Should().HaveCount(exampleGenreList.Count);
        output.Items.Select(x=>x.Id).Should().BeEquivalentTo(expectedOrderedList.Select(x=>x.Id), options => options.WithStrictOrdering());
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);

    }
}