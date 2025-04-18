﻿// --------------------------------------------------------------------------------------------------------------------
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

public class CategoryRepository : ICategoryRepository
{
    private readonly CodeflixCatalogDbContext dbContext;
    private DbSet<Category> Categories => this.dbContext.Set<Category>();

    public CategoryRepository(CodeflixCatalogDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Insert(Category video, CancellationToken cancellationToken)
      => await this.Categories.AddAsync(video, cancellationToken);

    public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
    {
        var category = await this.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == id, cancellationToken);

        if (category == null)
        {
            NotFoundException.ThrowIfNull(category,$"Category {id} not found.");
        }

        return category!;
    }

    public Task Delete(Category video, CancellationToken _)
        => Task.FromResult(this.Categories.Remove(video));

    public async Task Update(Category video, CancellationToken cancellationToken)
         => await Task.FromResult(this.Categories.Update(video));


    public async Task<SearchOutput<Category>> Search(SearchInput searchInput, CancellationToken cancellationToken)
    {
        var toSkip = searchInput.PerPage * (searchInput.Page - 1);
        var query =
            this.Categories
            .AsNoTracking();

      query = this.AddOrderToQuery(query, searchInput.OrderBy, searchInput.Order);

        if (string.IsNullOrWhiteSpace(searchInput.Search) is false)
        {
            query = query
                .Where(category => category.Name.Contains(searchInput.Search));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(toSkip)
            .Take(searchInput.PerPage)
            .ToListAsync(cancellationToken);


        return new SearchOutput<Category>(searchInput.Page,searchInput.PerPage, total, items);
    }

    public async Task<IReadOnlyList<Guid>> GetIdsByIds(List<Guid> ids, CancellationToken cancellationToken) => await this
        .Categories
        .AsNoTracking()
        .Where(x => ids.Contains(x.Id))
        .Select(x => x.Id)
        .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Category>> GetListByIds(List<Guid> ids, CancellationToken cancellationToken) =>
        await this.Categories
        .AsNoTracking()
        .Where(x => ids.Contains(x.Id))
        .ToListAsync(cancellationToken);

    private IQueryable<Category> AddOrderToQuery(IQueryable<Category> query, string orderProperty, SearchOrder order)
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