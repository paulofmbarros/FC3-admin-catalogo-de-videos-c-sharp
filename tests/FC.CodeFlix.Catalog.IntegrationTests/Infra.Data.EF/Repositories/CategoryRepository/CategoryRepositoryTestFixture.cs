// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

using Base;
using Catalog.Infra.Data.EF;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTestFixtureCollection : ICollectionFixture<CategoryRepositoryTestFixture>
{

}


public class CategoryRepositoryTestFixture : BaseFixture
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
        var category = new Category(this.GetValidCategoryName(), this.GetValidCategoryDescription(),
            this.GetRandomBoolean());
        return category;
    }




    public List<Category> GetExampleCategoriesList(int length = 10)
         => Enumerable.Range(0, length).Select(_ => this.GetExampleCategory()).ToList();


    public  List<Category> GetExampleCategoriesListWithNames (List<string> names)
        => names.Select(name => new Category(name, this.GetValidCategoryDescription(), this.GetRandomBoolean())).ToList();

    public List<Category> CloneCategoriesListOrdered(List<Category> categories,string orderBy, SearchOrder order)
    {
        var listCloned = new List<Category>(categories);
        var orderedList = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => listCloned.OrderBy(x => x.Name).ThenBy(x=>x.Id),
            ("name", SearchOrder.Desc) => listCloned.OrderByDescending(x => x.Name).ThenByDescending(x=>x.Id),
            ("id", SearchOrder.Asc) => listCloned.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listCloned.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listCloned.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listCloned.OrderByDescending(x => x.CreatedAt),
            _ => listCloned.OrderBy(x => x.Name).ThenBy(x=>x.Id)
        };

        return orderedList.ToList();
    }
}