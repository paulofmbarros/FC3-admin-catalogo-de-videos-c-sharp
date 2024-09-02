// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.Common;

using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Moq;
using UnitTets.Common;

public abstract class CategoryUseCasesBaseFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
        {
            categoryName = this.Faker.Commerce.Categories(1)[0];
        }

        if (categoryName.Length > 255)
        {
            categoryName = categoryName.Substring(0, 255);
        }

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = this.Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription.Substring(0, 10000);
        }

        return categoryDescription;
    }


    public bool GetRandomBoolean() => this.Faker.Random.Bool();

    public Category GetExampleCategory()
    {
        var category = new Category(this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());
        return category;
    }


    public Mock<ICategoryRepository> GetRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}