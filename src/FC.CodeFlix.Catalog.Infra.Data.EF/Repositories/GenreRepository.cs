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
        var toSkip = searchInput.PerPage * (searchInput.Page - 1);
        var query = this.Genres.AsNoTracking();

        query = this.AddOrderToQuery(query, searchInput.OrderBy, searchInput.Order);

        if (!string.IsNullOrWhiteSpace(searchInput.Search))
        {
            query = query.Where(x => x.Name.Contains(searchInput.Search));
        }

        var total = await query.CountAsync();

        var genres = await query
            .Skip(toSkip)
            .Take(searchInput.PerPage)
            .ToListAsync(cancellationToken);

        var genreIds = genres.Select(x => x.Id).ToList();

        var relations  = await this.GenresCategories
            .Where(relation => genreIds
                .Contains(relation.GenreId))
            .ToListAsync(cancellationToken);

        var relationsBeGenreIdGroup = relations.GroupBy(x => x.GenreId).ToList();

        foreach (var relationGroup in relationsBeGenreIdGroup)
        {
            var genre = genres.Find(x => x.Id == relationGroup.Key);

            foreach (var relation in relationGroup)
            {
                genre.AddCategory(relation.CategoryId);
            }
        }

        return new SearchOutput<Genre>( searchInput.Page, searchInput.PerPage, total, genres);
    }

    public Task<IReadOnlyList<Guid>> GetIdsByIds(List<Guid> ids, CancellationToken cancellationToken) => throw new NotImplementedException();


    private IQueryable<Genre> AddOrderToQuery(IQueryable<Genre> query, string orderProperty, SearchOrder order)
        => (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name).ThenBy(x=>x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name).ThenByDescending(x=>x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name).ThenBy(x=>x.Id),
        };
}