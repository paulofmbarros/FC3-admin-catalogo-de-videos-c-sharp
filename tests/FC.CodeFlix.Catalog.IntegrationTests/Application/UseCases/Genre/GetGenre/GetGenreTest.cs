// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

using Catalog.Infra.Data.EF.Models;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using FluentAssertions;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Integration/Application ", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var genres =  this.fixture.GetExampleGenresList();
        var expectedGenre = genres[5];

        var dbArrangeContext = fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genres);
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(fixture.CreateDbContext(true));

        var useCase = new GetGenre(genreRepository);

        var input = new  GetGenreInput(expectedGenre.Id);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);

    }

    [Fact(DisplayName = nameof(GetGenreThrowsWhenNotFound))]
    [Trait("Integration/Application ", "GetGenre - Use Cases")]
    public async Task GetGenreThrowsWhenNotFound()
    {
        var expectedGenre = Guid.NewGuid();

        var dbArrangeContext = fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync();
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(fixture.CreateDbContext(true));

        var useCase = new GetGenre(genreRepository);

        var input = new  GetGenreInput(expectedGenre);
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {expectedGenre} not found");

    }

    [Fact(DisplayName = nameof(GetGenreWithCategoryRelations))]
    [Trait("Integration/Application ", "GetGenre - Use Cases")]
    public async Task GetGenreWithCategoryRelations()
    {
        var genres =  this.fixture.GetExampleGenresList();
        var categoriesExampleList = this.fixture.GetExampleCategoriesList();
        var expectedGenre = genres[5];
        
        foreach (var category in categoriesExampleList)
        {
            expectedGenre.AddCategory(category.Id);
        }

        var dbArrangeContext = this.fixture.CreateDbContext();
        await dbArrangeContext.Categories.AddRangeAsync(categoriesExampleList);
        await dbArrangeContext.Genres.AddRangeAsync(genres);
        await dbArrangeContext.GenresCategories.AddRangeAsync(expectedGenre.Categories.Select(categoryId => new GenresCategories(categoryId, expectedGenre.Id)));
        await dbArrangeContext.SaveChangesAsync();
        var genreRepository = new GenreRepository(this.fixture.CreateDbContext(true));

        var useCase = new GetGenre(genreRepository);

        var input = new  GetGenreInput(expectedGenre.Id);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(expectedGenre.Id);
        output.Name.Should().Be(expectedGenre.Name);
        output.IsActive.Should().Be(expectedGenre.IsActive);
        output.CreatedAt.Should().Be(expectedGenre.CreatedAt);
        output.Categories.Should().HaveCount(expectedGenre.Categories.Count);

        foreach (var relationModel in output.Categories)
        {
            expectedGenre.Categories.Should().Contain(relationModel.Id);
            relationModel.Name.Should().BeNull();
        }

    }

}