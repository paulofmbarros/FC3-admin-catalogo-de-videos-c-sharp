// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

public class CategoryPersistence
{
    private readonly CodeflixCatalogDbContext dbContext;

    public CategoryPersistence(CodeflixCatalogDbContext dbContext) => this.dbContext = dbContext;

    public async Task<Category?> GetById(Guid id)
        => await this.dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
}