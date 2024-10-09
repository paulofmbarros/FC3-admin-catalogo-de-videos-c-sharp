// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Genre;

using Common;
using Fc.CodeFlix.Catalog.Domain.Entity;

[CollectionDefinition(nameof(GenreTestFixture))]
public class GenreTestFixtureCollection : ICollectionFixture<GenreTestFixture>
{

}

public class GenreTestFixture : BaseFixture
{
    public string GetValidName() => Faker.Commerce.Categories(1)[0];

    public Genre GetExampleGenre(bool isActive = true, List<Guid>? categoriesIds = null)
    {
        var genre = new Genre(this.GetValidName(), isActive);

        if (categoriesIds == null)
        {
            return genre;
        }

        foreach (var categoryId in categoriesIds)
        {
            genre.AddCategory(categoryId);
        }

        return genre;
    }

}