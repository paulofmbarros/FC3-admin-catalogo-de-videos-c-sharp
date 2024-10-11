// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.UpdateGenre;

using Fc.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;
using Moq;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTests
{
    private readonly UpdateGenreTestFixture fixture;

    public UpdateGenreTests(UpdateGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleGenre = this.fixture.GetExampleGenre();
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);


        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id ,this.fixture.GetValidGenreName(), this.fixture.GetRandomBoolean());

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Update(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(0);

        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x=>x.Update(It.Is<Genre>(g=>g.Id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);


    }
    
}