// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Models;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
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

    [Fact(DisplayName = nameof(GetThrowWhenNotFound))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetThrowWhenNotFound()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleNotFoundGuid = Guid.NewGuid();

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

        var action = async () => await genreRepository.Get(exampleNotFoundGuid, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {exampleNotFoundGuid} not found");

    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Delete()
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

        var repositoryDbContext = this.fixture.CreateDbContext(true);

        var genreRepository = new GenreRepository(repositoryDbContext);

        await genreRepository.Delete(exampleGenre, CancellationToken.None);

        await repositoryDbContext.SaveChangesAsync();

        var assertsDbContext = this.fixture.CreateDbContext(true);

        var dbGenre = await assertsDbContext.Genres.AsNoTracking().FirstOrDefaultAsync(x => x.Id == exampleGenre.Id);
        dbGenre.Should().BeNull();

        var genreCategoriesRelations = await assertsDbContext.GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .Select(x=>x.CategoryId)
            .ToListAsync();

        genreCategoriesRelations.Should().NotBeNull();
        genreCategoriesRelations.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Update()
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

        var actDbContext = this.fixture.CreateDbContext(true);

        var genreRepository = new GenreRepository(actDbContext);

        exampleGenre.Update(this.fixture.GetGenreName());

        if(exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }

        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();


        var assertContext = this.fixture.CreateDbContext(true);

        var genreFromDb = await assertContext.Genres.FindAsync(exampleGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);

        var genreCategoriesRelations = await assertContext.GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .Select(x=>x.CategoryId)
            .ToListAsync();

        genreCategoriesRelations.Should().NotBeNull();
        genreCategoriesRelations.Should().HaveCount(categoriesListExample.Count);

    }

    [Fact(DisplayName = nameof(UpdateRemovingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateRemovingRelations()
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

        var actDbContext = this.fixture.CreateDbContext(true);

        var genreRepository = new GenreRepository(actDbContext);

        exampleGenre.Update(this.fixture.GetGenreName());

        if(exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }

        exampleGenre.RemoveAllCategories();
        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();


        var assertContext = this.fixture.CreateDbContext(true);

        var genreFromDb = await assertContext.Genres.FindAsync(exampleGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);

        var genreCategoriesRelations = await assertContext.GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .Select(x=>x.CategoryId)
            .ToListAsync();

        genreCategoriesRelations.Should().NotBeNull();
        genreCategoriesRelations.Should().HaveCount(0);

    }

      [Fact(DisplayName = nameof(UpdateReplacingRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateReplacingRelations()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();

        // cria genero
        var exampleGenre = this.fixture.GetExampleGenre();

        // cria categorias que ficam associadas ao genero
        var categoriesListExample = this.fixture.GetExampleCategoriesList(3);
        var newCategoriesListExample = this.fixture.GetExampleCategoriesList(8);
        foreach (var category in categoriesListExample)
        {
            exampleGenre.AddCategory(category.Id);
        }

        await dbContext.Categories.AddRangeAsync(categoriesListExample);
        await dbContext.Categories.AddRangeAsync(newCategoriesListExample);
        await dbContext.Genres.AddAsync(exampleGenre);

        //adiciona as relações entre genero e categorias
        foreach (var categoryId in exampleGenre.Categories)
        {
            await dbContext.GenresCategories.AddAsync(new GenresCategories(categoryId, exampleGenre.Id));
        }

        await dbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var genreRepository = new GenreRepository(actDbContext);

        exampleGenre.Update(this.fixture.GetGenreName());

        if(exampleGenre.IsActive)
        {
            exampleGenre.Deactivate();
        }
        else
        {
            exampleGenre.Activate();
        }

        exampleGenre.RemoveAllCategories();
        foreach (var category in newCategoriesListExample)
        {
            exampleGenre.AddCategory(category.Id);
        }

        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync();


        var assertContext = this.fixture.CreateDbContext(true);

        var genreFromDb = await assertContext.Genres.FindAsync(exampleGenre.Id);

        genreFromDb.Should().NotBeNull();
        genreFromDb.Name.Should().Be(exampleGenre.Name);
        genreFromDb.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        genreFromDb.IsActive.Should().Be(exampleGenre.IsActive);

        var genreCategoriesRelations = await assertContext.GenresCategories
            .Where(r => r.GenreId == exampleGenre.Id)
            .Select(x=>x.CategoryId)
            .ToListAsync();

        genreCategoriesRelations.Should().NotBeNull();
        genreCategoriesRelations.Should().HaveCount(newCategoriesListExample.Count);

    }

      [Fact(DisplayName = nameof(ListReturnsItemsAndTotal))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task ListReturnsItemsAndTotal()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleGenresList = this.fixture.GetExampleGenresList(10);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync();
        var actDbContext = this.fixture.CreateDbContext(true);
        var genreRepository = new GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20,"", "", SearchOrder.Asc);


        var searchResult = await genreRepository.Search(searchInput,  CancellationToken.None);


        searchResult.Should().NotBeNull();
        searchResult.Items.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.Items.Select(x=>x.Id).Should().BeEquivalentTo(exampleGenresList.Select(x=>x.Id));

    }

    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsRelations()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleGenresList = this.fixture.GetExampleGenresList(10);
        foreach (var genre in exampleGenresList)
        {

            foreach (var category in exampleGenresList)
            {
                genre.AddCategory(category.Id);
            }

            var categoriesList = this.fixture.GetExampleCategoriesList(3);
            await dbContext.Categories.AddRangeAsync(categoriesList);
            var relationsToAdd = categoriesList.Select(x=>new GenresCategories(x.Id, genre.Id)).ToList();
            await dbContext.GenresCategories.AddRangeAsync(relationsToAdd);

        }

        await dbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var genreRepository = new GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20,"", "", SearchOrder.Asc);


        var searchResult = await genreRepository.Search(searchInput,  CancellationToken.None);


        searchResult.Should().NotBeNull();
        searchResult.Items.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(exampleGenresList.Count);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.Items.Select(x=>x.Id).Should().BeEquivalentTo(exampleGenresList.Select(x=>x.Id));

        foreach (var genre in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.FindAll(x=>x.Id == genre.Id);
            genre.Categories.Should().NotBeNull();
            genre.Categories.Should().HaveCount(3);
            genre.Categories.Should().BeEquivalentTo(exampleGenre);

        }




    }
}