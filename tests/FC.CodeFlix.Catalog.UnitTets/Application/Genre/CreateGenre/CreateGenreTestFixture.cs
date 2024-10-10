// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.CreateGenre;

using Common;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Moq;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture>
{

}


public class CreateGenreTestFixture : GenreUseCaseBaseFixture
{
    public CreateGenreInput GetExampleInput() => new CreateGenreInput(this.GetValidGenreName(), this.GetRandomBoolean());

    public CreateGenreInput GetExampleInputWithCategories()
    {
        var numberOfCategoriesId = new Random().Next(1, 10);
        var cateoriesId = Enumerable.Range(1, numberOfCategoriesId).Select(x => Guid.NewGuid()).ToList();

       return new CreateGenreInput(this.GetValidGenreName(), this.GetRandomBoolean(), cateoriesId);

    }

    public Mock<IGenreRepository> GetGenreRepositoryMock()
        => new() ;

    public Mock<IUnitOfWork> GetUnitOfWorkMock()
     => new();

    public Mock<ICategoryRepository> GetCategoryRepositoryMock()
        => new ();


}