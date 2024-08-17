// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CreateCategory;

using Domain.Common;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Moq;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture>
{
}


public class CreateCategoryTestFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
        {
            categoryName = Faker.Commerce.Categories(1)[0];
        }

        if (categoryName.Length > 255)
        {
            categoryName = categoryName.Substring(0, 255);
        }

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription.Substring(0, 10000);
        }

        return categoryDescription;
    }

    public bool GetRandomBoolean() => Faker.Random.Bool();

    public CreateCategoryInput GetInput() => new (this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());

    public Mock<ICategoryRepository> GetRepositoryMock => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock => new();

}