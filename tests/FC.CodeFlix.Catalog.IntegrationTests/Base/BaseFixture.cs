// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Base;

using Bogus;
using Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture() => this.Faker = new Faker("pt_BR");

    public CodeflixCatalogDbContext CreateDbContext(bool preserveDatabase = false)
    {
        var options = new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase("integration-tests-db")
            .Options;

        var dbContext = new CodeflixCatalogDbContext(options);

        if(preserveDatabase is false)
        {
            dbContext.Database.EnsureDeleted();
        }



        return dbContext;
    }
}