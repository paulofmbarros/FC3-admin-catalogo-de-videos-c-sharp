// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CastMember.UpdateCastMember;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;

[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
{

    [Fact(DisplayName = nameof(Update))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task Update()
    {
        var castMember = fixture.GetExampleCastMember();
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMember);

        repositoryMock.Setup(x => x.Update(castMember, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var updateCastMemberInput = new UpdateCastMemberInput(castMember.Id, fixture.GetValidName(), fixture.GetRandomCastMemberType() );
        var useCase = new UpdateCastMember(unitOfWorkMock.Object, repositoryMock.Object);
        var result = await useCase.Handle(updateCastMemberInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(castMember.Id);
        result.Name.Should().Be(updateCastMemberInput.Name);
        result.Type.Should().Be(updateCastMemberInput.Type);
        result.CreatedAt.Should().Be(castMember.CreatedAt);
        repositoryMock.Verify(x=>x.Get(It.Is<Guid>(id => id == updateCastMemberInput.Id), It.IsAny<CancellationToken>()), Times.Once());
        repositoryMock.Verify(x=>x.Update(It.Is<CastMember>(castMemberInput =>
                    castMemberInput.Id == updateCastMemberInput.Id &&
                    castMemberInput.Name == updateCastMemberInput.Name &&
                    castMemberInput.Type == updateCastMemberInput.Type),
            It.IsAny<CancellationToken>()),
            Times.Once());
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once());


    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Not Found"));


        unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var updateCastMemberInput = new UpdateCastMemberInput(new Guid(), fixture.GetValidName(), fixture.GetRandomCastMemberType() );
        var useCase = new UpdateCastMember(unitOfWorkMock.Object, repositoryMock.Object);
        var result = async () =>  await useCase.Handle(updateCastMemberInput, CancellationToken.None);

        await result.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(x=>x.Get(It.Is<Guid>(id => id == updateCastMemberInput.Id), It.IsAny<CancellationToken>()), Times.Once());
        repositoryMock.Verify(x=>x.Update(It.Is<CastMember>(castMemberInput =>
                    castMemberInput.Id == updateCastMemberInput.Id &&
                    castMemberInput.Name == updateCastMemberInput.Name &&
                    castMemberInput.Type == updateCastMemberInput.Type),
                It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Never);

    }

    [Fact(DisplayName = nameof(ThrowWhenInvalidName))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task ThrowWhenInvalidName()
    {
        var repositoryMock = new Mock<ICastMemberRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var castMember = fixture.GetExampleCastMember();
        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(castMember);

        unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var updateCastMemberInput = new UpdateCastMemberInput(castMember.Id, null, fixture.GetRandomCastMemberType() );
        var useCase = new UpdateCastMember(unitOfWorkMock.Object, repositoryMock.Object);
        var result = async () =>  await useCase.Handle(updateCastMemberInput, CancellationToken.None);

        await result
            .Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");

        repositoryMock.Verify(x=>x.Get(It.Is<Guid>(id => id == updateCastMemberInput.Id), It.IsAny<CancellationToken>()), Times.Once());
        repositoryMock.Verify(x=>x.Update(It.Is<CastMember>(castMemberInput =>
                    castMemberInput.Id == updateCastMemberInput.Id &&
                    castMemberInput.Name == updateCastMemberInput.Name &&
                    castMemberInput.Type == updateCastMemberInput.Type),
                It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Never);

    }

}