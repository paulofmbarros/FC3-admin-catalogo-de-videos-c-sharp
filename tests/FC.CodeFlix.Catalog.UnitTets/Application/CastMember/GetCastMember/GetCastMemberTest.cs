// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CastMember.GetCastMember;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;
using Fc.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;

[Collection(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTest
{
    private readonly GetCastMemberTestFixture fixture;

    public GetCastMemberTest(GetCastMemberTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = (nameof(Get)))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task Get()
    {
        // Arrange
        var castMember = this.fixture.GetExampleCastMember();
        var repositoryMock = new Mock<ICastMemberRepository>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMember);

        var input = new GetCastMemberInput(castMember.Id);
        var useCase = new GetCastMember(repositoryMock.Object);

        // Act
        var result = await useCase.Handle(input, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(castMember.Id);
        result.Name.Should().Be(castMember.Name);
        result.Type.Should().Be(castMember.Type);
        result.CreatedAt.Should().Be(castMember.CreatedAt);
        repositoryMock.Verify(x =>
            x.Get(
                It.Is<Guid>(id => id == input.Id),
                It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact(DisplayName = (nameof(ThrowIfCastMemberNotFound)))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task ThrowIfCastMemberNotFound()
    {
        // Arrange
        var repositoryMock = new Mock<ICastMemberRepository>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Not found"));

        var input = new GetCastMemberInput(Guid.NewGuid());
        var useCase = new GetCastMember(repositoryMock.Object);

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }
}