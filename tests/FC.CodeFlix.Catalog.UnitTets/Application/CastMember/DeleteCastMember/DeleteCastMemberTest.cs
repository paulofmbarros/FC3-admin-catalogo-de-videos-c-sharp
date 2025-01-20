// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CastMember.DeleteCastMember;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;

[Collection(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
{
    [Fact(DisplayName = (nameof(Delete)))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task Delete()
    {
        // Arrange
        var castMember = fixture.GetExampleCastMember();
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkRepository = new Mock<IUnitOfWork>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMember);

        var input = new DeleteCastMemberInput(castMember.Id);
        var useCase = new DeleteCastMember(repositoryMock.Object, unitOfWorkRepository.Object);

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().NotThrowAsync();

        repositoryMock.Verify(x =>
            x.Delete(
                It.Is<CastMember>(id => castMember.Id == input.Id),
                It.IsAny<CancellationToken>()),
            Times.Once());

        unitOfWorkRepository.Verify(x =>x.Commit(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact(DisplayName = (nameof(DeleteThrowsWhenNotFound)))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task DeleteThrowsWhenNotFound()
    {
        // Arrange
        var castMember = fixture.GetExampleCastMember();
        var repositoryMock = new Mock<ICastMemberRepository>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Not Found"));

        var input = new DeleteCastMemberInput(castMember.Id);
        var useCase = new DeleteCastMember(repositoryMock.Object, Mock.Of<IUnitOfWork>());

        // Act
        var action = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }

}