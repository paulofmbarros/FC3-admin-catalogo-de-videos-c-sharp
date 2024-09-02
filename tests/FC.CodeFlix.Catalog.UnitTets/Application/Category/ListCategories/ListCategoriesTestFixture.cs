// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.ListCategories;

using Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture>
{

}

public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
{

    public List<Category> GetExampleCategories(int length = 10)
    {
        var categories = new List<Category>();
        for (var i = 0; i < length; i++)
        {
            categories.Add(this.GetExampleCategory());
        }

        return categories;
    }

    public ListCategoriesInput GetExampleInput()
    {
        var random = new Random();
        return new ListCategoriesInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: this.Faker.Commerce.ProductName(),
            sort: this.Faker.Commerce.ProductName(),
            direction: random.Next(0,10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
    }
}