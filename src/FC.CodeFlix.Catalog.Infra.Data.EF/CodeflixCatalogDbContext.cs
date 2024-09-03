// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF;

using Configurations;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;

public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
    }
    
}