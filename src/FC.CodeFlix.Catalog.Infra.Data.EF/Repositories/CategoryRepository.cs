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
        var category = await this.Categories.FindAsync(new object[] {id}, cancellationToken);

        if (category == null)
        {
            NotFoundException.ThrowIfNull(category,$"Category '{id}' not found");
        }

        return category!;

    }

    public Task Delete(Category aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();

    public async Task Update(Category aggregate, CancellationToken cancellationToken)
         => await Task.FromResult(this.Categories.Update(aggregate));


    public Task<SearchOutput<Category>> Search(SearchInput searchInput, CancellationToken cancellationToken) => throw new NotImplementedException();
}