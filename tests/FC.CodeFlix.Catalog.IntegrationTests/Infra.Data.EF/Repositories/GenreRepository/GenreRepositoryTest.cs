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
        var categoriesList = this.fixture.GetExampleCategoriesList(3);

        await dbContext.Genres.AddRangeAsync(exampleGenresList);

        foreach (var genre in exampleGenresList)
        {

            foreach (var category in categoriesList)
            {
                genre.AddCategory(category.Id);
            }

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
            var exampleGenre = exampleGenresList.Find(x=>x.Id == genre.Id);
            genre.Categories.Should().NotBeNull();
            genre.Categories.Should().HaveCount(3);
            genre.Categories.Should().BeEquivalentTo(exampleGenre.Categories);

        }

    }

      [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
    {
        var actDbContext = this.fixture.CreateDbContext();
        var genreRepository = new GenreRepository(actDbContext);
        var searchInput = new SearchInput(1, 20,"", "", SearchOrder.Asc);


        var searchResult = await genreRepository.Search(searchInput,  CancellationToken.None);


        searchResult.Should().NotBeNull();
        searchResult.Items.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(0);
        searchResult.Total.Should().Be(0);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.CurrentPage.Should().Be(searchInput.Page);


    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsPaginated(int quantityToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleGenresList = this.fixture.GetExampleGenresList(quantityToGenerate);
        var categoriesList = this.fixture.GetExampleCategoriesList(3);

        await dbContext.Genres.AddRangeAsync(exampleGenresList);

        foreach (var genre in exampleGenresList)
        {

            foreach (var category in categoriesList)
            {
                genre.AddCategory(category.Id);
            }

            await dbContext.Categories.AddRangeAsync(categoriesList);
            var relationsToAdd = categoriesList.Select(x=>new GenresCategories(x.Id, genre.Id)).ToList();
            await dbContext.GenresCategories.AddRangeAsync(relationsToAdd);

        }

        await dbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var genreRepository = new GenreRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage,"", "", SearchOrder.Asc);


        var searchResult = await genreRepository.Search(searchInput,  CancellationToken.None);


        searchResult.Should().NotBeNull();
        searchResult.Items.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(expectedQuantityItems);
        searchResult.Total.Should().Be(exampleGenresList.Count);
        searchResult.PerPage.Should().Be(perPage);
        searchResult.CurrentPage.Should().Be(searchInput.Page);

        foreach (var genre in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x=>x.Id == genre.Id);
            genre.Categories.Should().NotBeNull();
            genre.Categories.Should().HaveCount(3);
            genre.Categories.Should().BeEquivalentTo(exampleGenre.Categories);

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
        var dbContext = this.fixture.CreateDbContext();
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
        var categoriesList = this.fixture.GetExampleCategoriesList(3);

        await dbContext.Genres.AddRangeAsync(exampleGenresList);

        foreach (var genre in exampleGenresList)
        {

            foreach (var category in categoriesList)
            {
                genre.AddCategory(category.Id);
            }

            await dbContext.Categories.AddRangeAsync(categoriesList);
            var relationsToAdd = categoriesList
                .Select(x=>new GenresCategories(x.Id, genre.Id)).ToList();
            await dbContext.GenresCategories.AddRangeAsync(relationsToAdd);

        }

        await dbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var genreRepository = new GenreRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);


        var searchResult = await genreRepository.Search(searchInput,  CancellationToken.None);


        searchResult.Should().NotBeNull();
        searchResult.Items.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);
        searchResult.Total.Should().Be(expectedQuantityTotalItems);
        searchResult.PerPage.Should().Be(perPage);
        searchResult.CurrentPage.Should().Be(searchInput.Page);

        foreach (var genre in searchResult.Items)
        {
            var exampleGenre = exampleGenresList.Find(x=>x.Id == genre.Id);
            genre.Categories.Should().NotBeNull();
            genre.Categories.Should().HaveCount(3);
            genre.Categories.Should().BeEquivalentTo(exampleGenre.Categories);

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
    public async Task SearchOrdered(string orderBy, string order)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleGenresList = this.fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new GenreRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1,20, "", orderBy, searchOrder);


        var output = await repository.Search(searchInput, CancellationToken.None);
        var expectedOrderedList = this.fixture.CloneGenresListOrdered(exampleGenresList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleGenresList.Count);
        output.Items.Should().HaveCount(exampleGenresList.Count);
        output.Items.Should().BeEquivalentTo(expectedOrderedList, options => options.WithStrictOrdering());
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }

    [Fact(DisplayName = nameof(GetIdsListByIds))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetIdsListByIds()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleGenresList = this.fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = this.fixture.CreateDbContext(true);
        var repository = new GenreRepository(actDbContext);
        var idsToGet = exampleGenresList.Select(x => x.Id).Take(2).ToList();

        var result = await repository.GetIdsByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(idsToGet.Count);
        result.ToList().Should().BeEquivalentTo(idsToGet);

    }

    [Fact(DisplayName = nameof(GetIdsListByIdsWhenOnlyThreeIdsMatch))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetIdsListByIdsWhenOnlyThreeIdsMatch()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleGenresList = this.fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = this.fixture.CreateDbContext(true);
        var repository = new GenreRepository(actDbContext);
        var idsToGet = exampleGenresList.Select(x => x.Id)
            .Take(3)
            .Concat(new []{Guid.NewGuid(), Guid.NewGuid(), })
            .ToList();

        var result = await repository.GetIdsByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.ToList().Should().NotBeEquivalentTo(idsToGet);
        idsToGet.Should().Contain(result);

    }

    [Fact(DisplayName = nameof(GetListByIds))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetListByIds()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleGenresList = this.fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = this.fixture.CreateDbContext(true);
        var repository = new GenreRepository(actDbContext);
        var idsToGet = exampleGenresList.Select(x => x.Id)
            .Take(3)
            .ToList();

        var result = await repository.GetListByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(idsToGet.Count);
        idsToGet.ForEach(id =>
        {
            var example = exampleGenresList.Find(x => x.Id == id);
            var resultItem = result.FirstOrDefault(x => x.Id == id);
            example.Should().NotBeNull();
            resultItem.Should().NotBeNull();
            resultItem.Name.Should().Be(example.Name);
            resultItem.Id.Should().Be(example.Id);
            resultItem.IsActive.Should().Be(example.IsActive);

        });

    }

    [Fact(DisplayName = nameof(GetListByIdsWhenOnlyThreeIdsMatch))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetListByIdsWhenOnlyThreeIdsMatch()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleGenresList = this.fixture.GetExampleGenresList(10);
        await dbContext.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = this.fixture.CreateDbContext(true);
        var repository = new GenreRepository(actDbContext);
        var idsToGet = exampleGenresList.Select(x => x.Id)
            .Take(3)
            .Concat(new []{Guid.NewGuid(), Guid.NewGuid(), })
            .ToList();

        var idsExpectedToGet = idsToGet.Take(3).ToList();

        var result = await repository.GetListByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        idsExpectedToGet.ForEach(id =>
        {
            var example = exampleGenresList.Find(x => x.Id == id);
            var resultItem = result.FirstOrDefault(x => x.Id == id);
            example.Should().NotBeNull();
            resultItem.Should().NotBeNull();
            resultItem.Name.Should().Be(example.Name);
            resultItem.Id.Should().Be(example.Id);
            resultItem.IsActive.Should().Be(example.IsActive);

        });

    }
}