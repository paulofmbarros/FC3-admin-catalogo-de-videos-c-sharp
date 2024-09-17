// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork;

using Catalog.Infra.Data.EF;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest
{

    private readonly UnitOfWorkTestFixture fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCategoryList = this.fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategoryList);

        var unitOfWork = new UnitOfWork(dbContext);

        await unitOfWork.Commit(CancellationToken.None);

        var assertDbContext = this.fixture.CreateDbContext(true);
        var categories = await assertDbContext.Categories.AsNoTracking().ToListAsync();

        categories.Should().HaveCount(exampleCategoryList.Count);
    }

    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = this.fixture.CreateDbContext();
        var unitOfWork = new UnitOfWork(dbContext);

        var task = async () => await unitOfWork.Rollback(CancellationToken.None);

        await task.Should().NotThrowAsync();
    }


}