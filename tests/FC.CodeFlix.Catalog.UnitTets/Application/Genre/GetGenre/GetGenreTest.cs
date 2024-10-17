// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.GetGenre;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;
using Moq;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task GetGenre()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();

        var exampleGenre = this.fixture.GetExampleGenre(categoriesIds: this.fixture.GetRandomIdsList());
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new GetGenre(genreRepositoryMock.Object);
        var input = new GetGenreInput(exampleGenre.Id);

        var output = await useCase.Handle(input, CancellationToken.None);


        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be((bool)exampleGenre.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(exampleGenre.Categories.Count());

        foreach (var expectedId in output.Categories)
        {
            exampleGenre.Categories.Should().Contain(x => x == expectedId);
        }


        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);


    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var exampleId = Guid.NewGuid();
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(x=>x == exampleId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre {exampleId} not found"));

        var useCase = new GetGenre(genreRepositoryMock.Object);
        var input = new GetGenreInput(exampleId);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {exampleId} not found");
        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(x=>x == exampleId), It.IsAny<CancellationToken>()), Times.Once);

    }

}