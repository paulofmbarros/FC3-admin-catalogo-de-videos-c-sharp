// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Models;
using Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleGenre = this.fixture.GetExampleGenre();
        var categoriesListExample = this.fixture.GetExampleCategoriesList(3);
        foreach (var category in categoriesListExample)
        {
            exampleGenre.AddCategory(category.Id);
        }

        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new GenreRepository(dbContext);

        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = this.fixture.CreateDbContext(true);

        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);
        dbGenre.Should().NotBeNull();
        dbGenre.Name.Should().Be(exampleGenre.Name);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);

        var genreCategoriesRelations = await assertsDbContext.GenresCategories.Where(r => r.GenreId == exampleGenre.Id).ToListAsync();

        genreCategoriesRelations.Should().NotBeNull();
        genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);
        genreCategoriesRelations.Select(r => r.CategoryId).Should().BeEquivalentTo(categoriesListExample.Select(c => c.Id));

    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();

        // cria genero
        var exampleGenre = this.fixture.GetExampleGenre();

        // cria categorias que ficam associadas ao genero
        var categoriesListExample = this.fixture.GetExampleCategoriesList(3);
        foreach (var category in categoriesListExample)
        {
            exampleGenre.AddCategory(category.Id);
        }

        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);

        //adiciona as relações entre genero e categorias
        foreach (var categoryId in exampleGenre.Categories)
        {
            await dbContext.GenresCategories.AddAsync(new GenresCategories(categoryId, exampleGenre.Id));
        }

        await dbContext.SaveChangesAsync();

        var genreRepository = new GenreRepository(this.fixture.CreateDbContext(true));

        var genreFromRepository = await genreRepository.Get(exampleGenre.Id, CancellationToken.None);

        genreFromRepository.Should().NotBeNull();
        genreFromRepository.Name.Should().Be(exampleGenre.Name);
        genreFromRepository.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreFromRepository.IsActive.Should().Be(exampleGenre.IsActive);

        genreFromRepository.Categories.Should().NotBeNull();
        genreFromRepository.Categories.Should().HaveCount(categoriesListExample.Count);
        genreFromRepository.Categories.Select(r => r).Should().BeEquivalentTo(categoriesListExample.Select(c => c.Id));

    }
}