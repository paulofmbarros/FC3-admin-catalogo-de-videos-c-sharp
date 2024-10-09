// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.CreateGenre;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Moq;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTests
{
    private readonly CreateGenreTestFixture fixture;

    public CreateGenreTests(CreateGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var useCase = new CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
        var input = this.fixture.GetExampleInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Insert(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.CreatedAt.Should().NotBeSameDateAs(default);


    }
}