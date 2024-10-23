// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;
using Models;

public class GenreRepository : IGenreRepository
{

    private readonly CodeflixCatalogDbContext dbContext;

    private DbSet<Genre> Genres => this.dbContext.Set<Genre>();

    private DbSet<GenresCategories> GenresCategories => this.dbContext.Set<GenresCategories>();

    public GenreRepository(CodeflixCatalogDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Insert(Genre genre, CancellationToken cancellationToken)
    {
        await this.Genres.AddAsync(genre, cancellationToken);

        if(genre.Categories.Count > 0)
        {
           var relations = genre.Categories.Select(categoryId => new GenresCategories(categoryId, genre.Id));
              await this.GenresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
    {
        var genre = await this.Genres.AsNoTracking().FirstOrDefaultAsync(x=>x.Id== id, cancellationToken);

        var categoryIds = await this.GenresCategories.Where(r => r.GenreId == id)
            .Select(x=>x.CategoryId)
            .ToListAsync(cancellationToken);

        NotFoundException.ThrowIfNull(genre, $"Genre {id} not found");

        foreach (var categoryId in categoryIds)
        {
            genre.AddCategory(categoryId);
        }

        return genre;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken)
    {
        this.GenresCategories.RemoveRange(this.GenresCategories.Where(x => x.GenreId == aggregate.Id));
        this.Genres.Remove(aggregate);

        return Task.CompletedTask;
    }

    public Task Update(Genre aggregate, CancellationToken cancellationToken)
    {
        this.Genres.Update(aggregate);
        //Remove pre-existing relations
        this.GenresCategories.RemoveRange(this.GenresCategories.Where(x => x.GenreId == aggregate.Id));

        //Add new relations
        this.GenresCategories.AddRange(aggregate.Categories.Select(categoryId => new GenresCategories(categoryId, aggregate.Id)));
        return Task.CompletedTask;
    }

    public async Task<SearchOutput<Genre>> Search(SearchInput searchInput, CancellationToken cancellationToken)
    {
        var genres = await this.Genres.ToListAsync(cancellationToken);
        return new SearchOutput<Genre>( searchInput.Page, searchInput.PerPage, genres.Count, genres);
    }
}