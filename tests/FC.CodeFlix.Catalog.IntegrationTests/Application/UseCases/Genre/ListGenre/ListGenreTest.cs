// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenre;

using Catalog.Infra.Data.EF.Models;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.ListGenre;
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


        var useCase = new ListGenres(new GenreRepository(this.fixture.CreateDbContext(true)));

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
        var useCase = new ListGenres(new GenreRepository(this.fixture.CreateDbContext()));

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


        var useCase = new ListGenres(new GenreRepository(this.fixture.CreateDbContext(true)));

        var input = new ListGenresInput(1, 20);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(exampleGenre.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleGenre.Count);
        output.Items.Select(x=>x.Id).Should().BeEquivalentTo(exampleGenre.Select(x=>x.Id));
        foreach (var item in output.Items)
        {
            var outputItemCategoriesIds = item.Categories.Select(c => c.Id).ToList();
            outputItemCategoriesIds.Should().BeEquivalentTo(exampleGenre.First(genre => genre.Id == item.Id).Categories.ToList().Select(id=>id));
        }

    }
}