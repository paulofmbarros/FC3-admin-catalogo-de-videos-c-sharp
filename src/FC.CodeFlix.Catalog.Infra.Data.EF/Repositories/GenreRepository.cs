// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;

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
        var genres = await this.Genres.FindAsync(id, cancellationToken);
        var categoryIds = await this.GenresCategories.Where(r => r.GenreId == id)
            .Select(x=>x.CategoryId)
            .ToListAsync(cancellationToken);

        foreach (var categoryId in categoryIds)
        {
            genres.AddCategory(categoryId);
        }

        return genres;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task Update(Genre aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<SearchOutput<Genre>> Search(SearchInput searchInput, CancellationToken cancellationToken) => throw new NotImplementedException();
}