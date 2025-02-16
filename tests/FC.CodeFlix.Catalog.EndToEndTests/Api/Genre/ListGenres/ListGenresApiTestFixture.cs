﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.ListGenres;

using Common;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Xunit;

[CollectionDefinition(nameof(ListGenresApiTestFixture))]
public class ListGenresApiTestFixtureCollection : ICollectionFixture<ListGenresApiTestFixture>
{

}


public class ListGenresApiTestFixture : GenreBaseFixture
{
    public  List<Genre> GetExampleCategoriesListWithNames (List<string> names)
        => names.Select(name => new Genre(name,  this.GetRandomBoolean())).ToList();

    public List<Genre> CloneGenresListOrdered(List<Genre> genres,string orderBy, SearchOrder order)
    {
        var listCloned = new List<Genre>(genres);
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