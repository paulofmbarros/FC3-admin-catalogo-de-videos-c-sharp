// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.Common;

using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Moq;
using UnitTets.Common;

public class GenreUseCaseBaseFixture : BaseFixture
{
    public string GetValidGenreName() => Faker.Commerce.Categories(1)[0];

    public Mock<IGenreRepository> GetGenreRepositoryMock()
        => new() ;

    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();

    public Mock<ICategoryRepository> GetCategoryRepositoryMock()
        => new ();

    public Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
    {
        var genre = new Genre(this.GetValidGenreName(), isActive ?? this.GetRandomBoolean());
        if (categoriesIds == null)
        {
            return genre;
        }

        foreach (var categoriesId in categoriesIds)
        {
            genre.AddCategory(categoriesId);
        }

        return genre;
    }

    public List<Guid> GetRandomIdsList(int? count = null)
        => Enumerable.Range(0, count ?? new Random().Next(1,10)).Select(x => Guid.NewGuid()).ToList();


}