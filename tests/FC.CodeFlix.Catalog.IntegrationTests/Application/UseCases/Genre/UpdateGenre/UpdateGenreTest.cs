// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.UpdateGenre;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Models;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTest
{
    private readonly UpdateGenreTestFixture fixture;

    public UpdateGenreTest(UpdateGenreTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Infrastructure/Application ", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        var exampleGenreList = this.fixture.GetExampleGenresList(10);
        var arrangeDbContext = this.fixture.CreateDbContext();
        var targetGenre = exampleGenreList.FirstOrDefault();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var useCase = new UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));
        
        var input = new UpdateGenreInput(targetGenre.Id, this.fixture.GetGenreName(), !targetGenre.IsActive);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);

        var assertDbContext = this.fixture.CreateDbContext(true);
        var genre = await assertDbContext.Genres.FindAsync(output.Id);
        genre.Should().NotBeNull();
        genre.Name.Should().Be(input.Name);
        genre.IsActive.Should().Be((bool)input.IsActive);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithCategoriesRelations))]
    [Trait("Infrastructure/Application ", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithCategoriesRelations()
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(10);
        var exampleGenreList = this.fixture.GetExampleGenresList(10);
        var arrangeDbContext = this.fixture.CreateDbContext();
        var targetGenre = exampleGenreList.FirstOrDefault();
        var relatedCategories = exampleCategories.Take(5).ToList();
        var newRelatedCategories = exampleCategories.Skip(5).Take(3).ToList();
        foreach (var category in relatedCategories)
        {
            targetGenre.AddCategory(category.Id);
        }

        var relation = targetGenre.Categories
            .Select(categoryId => new GenresCategories(targetGenre.Id, categoryId))
            .ToList();

        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.GenresCategories.AddRangeAsync(relation);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var useCase = new UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

        var input = new UpdateGenreInput(targetGenre.Id,
            this.fixture.GetGenreName(),
            !targetGenre.IsActive,
            newRelatedCategories.Select(x => x.Id)
                .ToList()
            );
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.Categories.Should().HaveCount(newRelatedCategories.Count);
        output.Categories.Select(relation => relation.Id)
            .Should()
            .BeEquivalentTo(input.CategoriesIds);

        var assertDbContext = this.fixture.CreateDbContext(true);
        var genre = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genre.Should().NotBeNull();
        genre.Name.Should().Be(input.Name);
        genre.IsActive.Should().Be((bool)input.IsActive);

        var relations = await assertDbContext.GenresCategories
            .Where(x => x.GenreId == input.Id)
            .ToListAsync();
        relations.Should().NotBeNull();
        relations.Select(x => x.CategoryId)
            .Should()
            .BeEquivalentTo(input.CategoriesIds);
        relations.Should().HaveCount(newRelatedCategories.Count);

    }


    [Fact(DisplayName = nameof(UpdateGenreThrowsWhenCategoryDoesntExist))]
    [Trait("Infrastructure/Application ", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreThrowsWhenCategoryDoesntExist()
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(10);
        var exampleGenreList = this.fixture.GetExampleGenresList(10);
        var arrangeDbContext = this.fixture.CreateDbContext();
        var targetGenre = exampleGenreList.FirstOrDefault();
        var relatedCategories = exampleCategories.Take(5).ToList();
        var newRelatedCategories = exampleCategories.Skip(5).Take(3).ToList();
        foreach (var category in relatedCategories)
        {
            targetGenre.AddCategory(category.Id);
        }

        var relation = targetGenre.Categories
            .Select(categoryId => new GenresCategories(targetGenre.Id, categoryId))
            .ToList();

        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.GenresCategories.AddRangeAsync(relation);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var useCase = new UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

        var categoriesIdsToRelate = newRelatedCategories.Select(x => x.Id).ToList();

        categoriesIdsToRelate.Add(Guid.NewGuid());

        var input = new UpdateGenreInput(targetGenre.Id,
            this.fixture.GetGenreName(),
            !targetGenre.IsActive,
            categoriesIdsToRelate
            );
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category Id (or ids) not found: {categoriesIdsToRelate.LastOrDefault()}");


    }

    [Fact(DisplayName = nameof(UpdateGenreThrowsWWhenNotFound))]
    [Trait("Infrastructure/Application ", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreThrowsWWhenNotFound()
    {
        var exampleGenreList = this.fixture.GetExampleGenresList(10);
        var arrangeDbContext = this.fixture.CreateDbContext();
        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var useCase = new UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

        Guid randomGuid = Guid.NewGuid();
        var input = new UpdateGenreInput(randomGuid, this.fixture.GetGenreName(), true);
        var action = async () => await useCase.Handle(input, CancellationToken.None);


        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Genre {randomGuid} not found");
    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutNewCategoriesRelations))]
    [Trait("Infrastructure/Application ", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutNewCategoriesRelations()
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(10);
        var exampleGenreList = this.fixture.GetExampleGenresList(10);
        var arrangeDbContext = this.fixture.CreateDbContext();
        var targetGenre = exampleGenreList.FirstOrDefault();
        var relatedCategories = exampleCategories.Take(5).ToList();
        foreach (var category in relatedCategories)
        {
            targetGenre.AddCategory(category.Id);
        }

        var relation = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();

        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.GenresCategories.AddRangeAsync(relation);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var useCase = new UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

        var input = new UpdateGenreInput(targetGenre.Id,
            this.fixture.GetGenreName(),
            !targetGenre.IsActive
            );
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.Categories.Should().HaveCount(relatedCategories.Count);
        output.Categories.Select(relation => relation.Id)
            .Should()
            .BeEquivalentTo(relatedCategories.Select(x => x.Id));

        var assertDbContext = this.fixture.CreateDbContext(true);
        var genre = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genre.Should().NotBeNull();
        genre.Name.Should().Be(input.Name);
        genre.IsActive.Should().Be((bool)input.IsActive);

        var relations = await assertDbContext.GenresCategories
            .Where(x => x.GenreId == input.Id)
            .ToListAsync();
        relations.Should().NotBeNull();
        relations.Select(x => x.CategoryId)
            .Should()
            .BeEquivalentTo(relatedCategories.Select(x => x.Id));
        relations.Should().HaveCount(relatedCategories.Count);

    }

     [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoryIdsCleanRelations))]
    [Trait("Infrastructure/Application ", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithEmptyCategoryIdsCleanRelations()
    {
        var exampleCategories = this.fixture.GetExampleCategoriesList(10);
        var exampleGenreList = this.fixture.GetExampleGenresList(10);
        var arrangeDbContext = this.fixture.CreateDbContext();
        var targetGenre = exampleGenreList.FirstOrDefault();
        var relatedCategories = exampleCategories.Take(5).ToList();
        foreach (var category in relatedCategories)
        {
            targetGenre.AddCategory(category.Id);
        }

        var relation = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();

        await arrangeDbContext.Genres.AddRangeAsync(exampleGenreList);
        await arrangeDbContext.Categories.AddRangeAsync(exampleCategories);
        await arrangeDbContext.GenresCategories.AddRangeAsync(relation);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var useCase = new UpdateGenre(new GenreRepository(actDbContext), new UnitOfWork(actDbContext), new CategoryRepository(actDbContext));

        var input = new UpdateGenreInput(targetGenre.Id,
            this.fixture.GetGenreName(),
            !targetGenre.IsActive,
            new List<Guid>()
            );
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(targetGenre.Id);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.Categories.Should().HaveCount(0);

        var assertDbContext = this.fixture.CreateDbContext(true);
        var genre = await assertDbContext.Genres.FindAsync(targetGenre.Id);
        genre.Should().NotBeNull();
        genre.Name.Should().Be(input.Name);
        genre.IsActive.Should().Be((bool)input.IsActive);

        var relations = await assertDbContext.GenresCategories
            .Where(x => x.GenreId == input.Id)
            .ToListAsync();
        relations.Should().NotBeNull();
        relations.Select(x => x.CategoryId)
            .Should()
            .BeEquivalentTo(new List<Guid>());
        relations.Should().HaveCount(0);

    }
}