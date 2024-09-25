// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.Common;

using Base;
using Fc.CodeFlix.Catalog.Domain.Entity;

public class CategoryBaseFixture : BaseFixture
{
    public CategoryPersistence CategoryPersistence { get; }

    public CategoryBaseFixture()
    {
        this.CategoryPersistence = new CategoryPersistence(this.CreateDbContext());
    }

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

    public string GetInvalidNameTooShort()
        => Faker.Commerce.ProductName().Substring(0, 2);

    public string GetInvalidNameTooLong()
    {
        var tooLongNameForCategory = Faker.Commerce.ProductName();
        while (tooLongNameForCategory.Length <= 255)
        {
            tooLongNameForCategory = $"{tooLongNameForCategory} {this.Faker.Commerce.ProductName()}";
        }

        return tooLongNameForCategory;
    }

    public string GetInvalidDescriptionTooLong()
    {
        var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();
        while (tooLongDescriptionForCategory.Length <= 10_000)
            tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductDescription()}";
        return tooLongDescriptionForCategory;
    }

    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            this.GetRandomBoolean()
        );

    public List<Category> GetExampleCategoriesList(int listLength = 15)
        => Enumerable.Range(1, listLength).Select(
            _ => new Category(
                GetValidCategoryName(),
                GetValidCategoryDescription(),
                this.GetRandomBoolean()
            )
        ).ToList();


    public bool GetRandomBoolean() => this.Faker.Random.Bool();
}