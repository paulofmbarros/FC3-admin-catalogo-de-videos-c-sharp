// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.DeleteGenre;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.DeleteGenre;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;
using Moq;

[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task DeleteGenre()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var exampleGenre = this.fixture.GetExampleGenre();
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);


        var useCase = new DeleteGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
        var input = new DeleteGenreInput(exampleGenre.Id);

        await useCase.Handle(input, CancellationToken.None);

        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=> id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x=>x.Delete(It.Is<Genre>(genre=>genre.Id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);


    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "DeleteGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var exampleId = Guid.NewGuid();
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(x=>x == exampleId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre {exampleId} not found"));

        var useCase = new DeleteGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
        var input = new DeleteGenreInput(exampleId);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {exampleId} not found");
        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(x=>x == exampleId), It.IsAny<CancellationToken>()), Times.Once);

    }
}