// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.CreateGenre;

using Common;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Moq;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture>
{

}


public class CreateGenreTestFixture : GenreUseCaseBaseFixture
{
    public CreateGenreInput GetExampleInput() => new CreateGenreInput(this.GetValidGenreName(), this.GetRandomBoolean());

    public Mock<IGenreRepository> GetGenreRepositoryMock()
    {
        var genreRepositoryMock = new Mock<IGenreRepository>();
        genreRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Genre>()))
            .Returns(Task.CompletedTask);
        return genreRepositoryMock;
    }

    public Mock<IUnitOfWork> GetUnitOfWorkMock()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(x => x.Commit(new CancellationToken()))
            .Returns(Task.CompletedTask);
        return unitOfWorkMock;
    }
}