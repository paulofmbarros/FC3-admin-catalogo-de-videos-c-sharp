// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

using Microsoft.EntityFrameworkCore;

[Collection(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTests
{
    private readonly CategoryRepositoryTestFixture fixture;

    public CategoryRepositoryTests(CategoryRepositoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = fixture.CreateDbContext();
        var exampleCategory = fixture.GetExampleCategory();

        var categoryRepository = new CategoryRepository(dbContext);

        await categoryRepository.Insert(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbCategory = await dbContext.Categories.Find(exampleCategory.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
    }

}