// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTest
{
    private readonly CreateGenreTestFixture fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        var input = this.fixture.GetExampleInput();

        var dbContext = this.fixture.CreateDbContext();
        var createGenre = new CreateGenre(new GenreRepository(dbContext), new UnitOfWork(dbContext), new CategoryRepository(dbContext));

        var output = await createGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);

        var assertDbContext = this.fixture.CreateDbContext(true);

        var genre = await assertDbContext.Genres.FindAsync(output.Id);
        genre.Should().NotBeNull();
        genre.Name.Should().Be(input.Name);
        genre.IsActive.Should().Be(input.IsActive);


    }


    [Fact(DisplayName = nameof(CreateGenreWithCategoryRelation))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateGenreWithCategoryRelation()
    {

        var categories = this.fixture.GetExampleCategoriesList(5);

        var arrangeDbContext = this.fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(categories);
        await arrangeDbContext.SaveChangesAsync();

        var input = this.fixture.GetExampleInput();

        input.Categories = categories.Select(x => x.Id).ToList();


        var dbContext = this.fixture.CreateDbContext(true);
        var createGenre = new CreateGenre(new GenreRepository(dbContext), new UnitOfWork(dbContext), new CategoryRepository(dbContext));

        var output = await createGenre.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().NotBeNull();
        output.Categories.Should().HaveCount(input.Categories.Count);
        output.Categories.Select(relation=>relation.Id).Should().BeEquivalentTo(input.Categories);
        output.CreatedAt.Should().NotBeSameDateAs(default);

        var assertDbContext = this.fixture.CreateDbContext(true);

        var genreFromDb = await assertDbContext.Genres.FindAsync(output.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive);

        var relations = await assertDbContext.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == output.Id)
            .ToListAsync();

        relations.Should().HaveCount(input.Categories.Count);
        relations.Select(x => x.CategoryId).Should().BeEquivalentTo(input.Categories);

    }

    [Fact(DisplayName = nameof(CreateGenreThrowsWhenCategoryDoesntExist))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateGenreThrowsWhenCategoryDoesntExist()
    {

        var categories = this.fixture.GetExampleCategoriesList(5);

        var arrangeDbContext = this.fixture.CreateDbContext();
        await arrangeDbContext.Categories.AddRangeAsync(categories);
        await arrangeDbContext.SaveChangesAsync();

        var input = this.fixture.GetExampleInput();

        input.Categories = categories.Select(x => x.Id).ToList();
        input.Categories.Add(Guid.NewGuid());

        var dbContext = this.fixture.CreateDbContext(true);
        var createGenre = new CreateGenre(new GenreRepository(dbContext), new UnitOfWork(dbContext), new CategoryRepository(dbContext));

        var action = async () => await createGenre.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category Id (or ids) not found: {input.Categories.Last()}");

    }
}