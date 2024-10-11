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

    public Genre GetExampleGenre()
        => new Genre(this.GetValidGenreName(), this.GetRandomBoolean());
}