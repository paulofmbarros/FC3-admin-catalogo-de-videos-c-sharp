// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.DeleteGenre;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Models;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTests
{
    private readonly DeleteGenreTestFixture fixture;

    public DeleteGenreTests(DeleteGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Integration/Application ", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var genres =  this.fixture.GetExampleGenresList();
        var targetGenre = genres[5];

        var dbArrangeContext = fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genres);
        await dbArrangeContext.SaveChangesAsync();


        var actDbContext = this.fixture.CreateDbContext(true);
        var genreRepository = new GenreRepository(actDbContext);

        var unitOfWork = new UnitOfWork(actDbContext);

        var useCase = new DeleteGenre(genreRepository, unitOfWork);

        var input = new DeleteGenreInput(targetGenre.Id);
       await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = this.fixture.CreateDbContext(true);
        var genre = await assertDbContext.Genres.FindAsync(targetGenre.Id);

        genre.Should().BeNull();

    }

    [Fact(DisplayName = nameof(DeleteGenreThrowsWhenNotFound))]
    [Trait("Integration/Application ", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreThrowsWhenNotFound()
    {
        var genres =  this.fixture.GetExampleGenresList();
        var targetGenre = genres[5];

        var dbArrangeContext = fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genres);
        await dbArrangeContext.SaveChangesAsync();


        var actDbContext = this.fixture.CreateDbContext(true);
        var genreRepository = new GenreRepository(actDbContext);
        var unitOfWork = new UnitOfWork(actDbContext);
        var useCase = new DeleteGenre(genreRepository, unitOfWork);
        var randomId = Guid.NewGuid();

        var input = new DeleteGenreInput(randomId);
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Genre {randomId} not found");


    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("Integration/Application ", "DeleteGenre - Use Cases")]
    public async Task DeleteGenreWithRelations()
    {
        var genres =  this.fixture.GetExampleGenresList();
        var targetGenre = genres[5];
        var exampleCategories = this.fixture.GetExampleCategoriesList(5);

        var dbArrangeContext = this.fixture.CreateDbContext();
        await dbArrangeContext.Genres.AddRangeAsync(genres);
        await dbArrangeContext.Categories.AddRangeAsync(exampleCategories);
        await dbArrangeContext.GenresCategories.AddRangeAsync(exampleCategories.Select(x => new GenresCategories(x.Id, targetGenre.Id)));
        await dbArrangeContext.SaveChangesAsync();


        var actDbContext = this.fixture.CreateDbContext(true);
        var genreRepository = new GenreRepository(actDbContext);

        var unitOfWork = new UnitOfWork(actDbContext);

        var useCase = new DeleteGenre(genreRepository, unitOfWork);

        var input = new DeleteGenreInput(targetGenre.Id);
        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = this.fixture.CreateDbContext(true);
        var genreFromDb = await assertDbContext.Genres.FindAsync(targetGenre.Id);

        genreFromDb.Should().BeNull();

        var relations = await assertDbContext.GenresCategories
            .Where(x => x.GenreId == targetGenre.Id)
            .ToListAsync();

        relations.Should().BeEmpty();

    }


}