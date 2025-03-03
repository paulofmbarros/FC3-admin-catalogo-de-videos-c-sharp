// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.DeleteVideo;

using System.ComponentModel;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.DeleteVideo;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;

[Collection(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTests
{
    private readonly DeleteVideoTestFixture fixture;
    private readonly DeleteVideo useCase;
    private readonly Mock<IVideoRepository> videoRepositoryMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IStorageService> storageServiceMock;

    public DeleteVideoTests(DeleteVideoTestFixture fixture)
    {
        this.fixture = fixture;
        this.videoRepositoryMock = new Mock<IVideoRepository>();
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.storageServiceMock = new Mock<IStorageService>();
        this.useCase = new DeleteVideo(videoRepositoryMock.Object, this.unitOfWorkMock.Object, storageServiceMock.Object);

    }

    [Fact(DisplayName = nameof(DeleteVideo))]
    [Trait("Application ", "Delete Video - Use Cases")]
    public async Task DeleteVideo()
    {
        var videoExample = fixture.GetValidVideo();

        var input = this.fixture.GetValidInput(videoExample.Id);

        this.videoRepositoryMock.Setup(x=> x.Get(It.Is<Guid>(id => id == videoExample.Id), It.IsAny<CancellationToken>())).ReturnsAsync(videoExample);

        await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepositoryMock.VerifyAll();
        this.videoRepositoryMock.Verify(x=> x.Delete(It.Is<Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once());
        this.unitOfWorkMock.Verify(x=> x.Commit(It.IsAny<CancellationToken>()), Times.Once());

    }

    [Fact(DisplayName = nameof(DeleteVideoWithAllMediasAndClearStorage))]
    [Trait("Application ", "Delete Video - Use Cases")]
    public async Task DeleteVideoWithAllMediasAndClearStorage()
    {
        var videoExample = fixture.GetValidVideo();
        videoExample.UpdateMedia(this.fixture.GetValidMediaPath());
        videoExample.UpdateTrailer(this.fixture.GetValidMediaPath());
        var filePaths = new List<string> { videoExample.Media.FilePath, videoExample.Trailer.FilePath };


        var input = this.fixture.GetValidInput(videoExample.Id);

        this.videoRepositoryMock.Setup(x=> x.Get(It.Is<Guid>(id => id == videoExample.Id), It.IsAny<CancellationToken>())).ReturnsAsync(videoExample);

        await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepositoryMock.VerifyAll();
        this.videoRepositoryMock.Verify(x=> x.Delete(It.Is<Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once());
        this.unitOfWorkMock.Verify(x=> x.Commit(It.IsAny<CancellationToken>()), Times.Once());
        this.storageServiceMock.Verify(x=> x.Delete(It.Is<string>(filePath =>filePaths.Contains(filePath)), It.IsAny<CancellationToken>()), Times.Exactly(2));
        this.storageServiceMock.Verify(x=> x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyTrailerAndClearStorageOnlyForTrailer))]
    [Trait("Application ", "Delete Video - Use Cases")]
    public async Task DeleteVideoWithOnlyTrailerAndClearStorageOnlyForTrailer()
    {
        var videoExample = fixture.GetValidVideo();
        videoExample.UpdateTrailer(this.fixture.GetValidMediaPath());

        var input = this.fixture.GetValidInput(videoExample.Id);

        this.videoRepositoryMock.Setup(x=> x.Get(It.Is<Guid>(id => id == videoExample.Id), It.IsAny<CancellationToken>())).ReturnsAsync(videoExample);

        await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepositoryMock.VerifyAll();
        this.videoRepositoryMock.Verify(x=> x.Delete(It.Is<Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once());
        this.unitOfWorkMock.Verify(x=> x.Commit(It.IsAny<CancellationToken>()), Times.Once());
        this.storageServiceMock.Verify(x=> x.Delete(It.Is<string>(filePath => filePath == videoExample.Trailer!.FilePath), It.IsAny<CancellationToken>()), Times.Exactly(1));
        this.storageServiceMock.Verify(x=> x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyMediaAndClearStorageOnlyForTrailer))]
    [Trait("Application ", "Delete Video - Use Cases")]
    public async Task DeleteVideoWithOnlyMediaAndClearStorageOnlyForTrailer()
    {
        var videoExample = fixture.GetValidVideo();
        videoExample.UpdateMedia(this.fixture.GetValidMediaPath());

        var input = this.fixture.GetValidInput(videoExample.Id);

        this.videoRepositoryMock.Setup(x=> x.Get(It.Is<Guid>(id => id == videoExample.Id), It.IsAny<CancellationToken>())).ReturnsAsync(videoExample);

        await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepositoryMock.VerifyAll();
        this.videoRepositoryMock.Verify(x=> x.Delete(It.Is<Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once());
        this.unitOfWorkMock.Verify(x=> x.Commit(It.IsAny<CancellationToken>()), Times.Once());
        this.storageServiceMock.Verify(x=> x.Delete(It.Is<string>(filePath => filePath == videoExample.Media!.FilePath), It.IsAny<CancellationToken>()), Times.Exactly(1));
        this.storageServiceMock.Verify(x=> x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

    }

    [Fact(DisplayName = nameof(DeleteVideoWithoutAnyMediaAndClearStorageOnlyForTrailer))]
    [Trait("Application ", "Delete Video - Use Cases")]
    public async Task DeleteVideoWithoutAnyMediaAndClearStorageOnlyForTrailer()
    {
        var videoExample = fixture.GetValidVideo();
        var input = this.fixture.GetValidInput(videoExample.Id);

        this.videoRepositoryMock.Setup(x=> x.Get(It.Is<Guid>(id => id == videoExample.Id), It.IsAny<CancellationToken>())).ReturnsAsync(videoExample);

        await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepositoryMock.VerifyAll();
        this.videoRepositoryMock.Verify(x=> x.Delete(It.Is<Video>(x => x.Id == videoExample.Id), It.IsAny<CancellationToken>()), Times.Once());
        this.unitOfWorkMock.Verify(x=> x.Commit(It.IsAny<CancellationToken>()), Times.Once());
        this.storageServiceMock.Verify(x=> x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

    }

    [Fact(DisplayName = nameof(ThrowsNotFoundExceptionWhenVideoNotFound))]
    [Trait("Application ", "Delete Video - Use Cases")]
    public async Task ThrowsNotFoundExceptionWhenVideoNotFound()
    {
        var input = this.fixture.GetValidInput();

        this.videoRepositoryMock.Setup(x=> x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException("Video not found"));

        var action = async () => await this.useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video not found");

        this.videoRepositoryMock.VerifyAll();
        this.videoRepositoryMock.Verify(x=> x.Delete(It.IsAny<Video>(), It.IsAny<CancellationToken>()), Times.Never);
        this.unitOfWorkMock.Verify(x=> x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        this.storageServiceMock.Verify(x=> x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

    }
}