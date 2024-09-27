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

public class CategoryRepository : ICategoryRepository
{
    private readonly CodeflixCatalogDbContext dbContext;
    private DbSet<Category> Categories => this.dbContext.Set<Category>();

    public CategoryRepository(CodeflixCatalogDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Insert(Category aggregate, CancellationToken cancellationToken)
      => await this.Categories.AddAsync(aggregate, cancellationToken);

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

    public Task Delete(Category aggregate, CancellationToken _)
        => Task.FromResult(this.Categories.Remove(aggregate));

    public async Task Update(Category aggregate, CancellationToken cancellationToken)
         => await Task.FromResult(this.Categories.Update(aggregate));


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

    private IQueryable<Category> AddOrderToQuery(IQueryable<Category> query, string orderProperty, SearchOrder order)
       => (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name),
        };
}