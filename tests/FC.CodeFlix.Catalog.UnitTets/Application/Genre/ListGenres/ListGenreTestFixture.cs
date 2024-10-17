// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.ListGenres;

using Common;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

[CollectionDefinition(nameof(ListGenreTestFixture))]
public class ListGenresTestFixtureCollection : ICollectionFixture<ListGenreTestFixture>
{

}

public class ListGenreTestFixture : GenreUseCaseBaseFixture
{
    public ListGenresInput GetListGenresInput()
    {
            var random = new Random();
            return new ListGenresInput(
                page: random.Next(1, 10),
                perPage: random.Next(15, 100),
                search: this.Faker.Commerce.ProductName(),
                sort: this.Faker.Commerce.ProductName(),
                direction: random.Next(0,10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
            );
    }
    
}