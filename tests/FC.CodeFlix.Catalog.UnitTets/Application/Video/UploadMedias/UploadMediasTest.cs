// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.UploadMedias;

using Fc.CodeFlix.Catalog.Application.Common;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;

[Collection(nameof(UploadMediasTestFixture))]
public class UploadMediasTest
{

    private readonly UploadMediasTestFixture fixture;
    private readonly UploadMedias useCase;
    private readonly Mock<IVideoRepository> videoRepository;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly Mock<IStorageService> storageService;

    public UploadMediasTest(UploadMediasTestFixture fixture)
    {
        this.fixture = fixture;
        this.videoRepository = new Mock<IVideoRepository>();
        this.unitOfWork = new Mock<IUnitOfWork>();
        this.storageService = new Mock<IStorageService>();
        this.useCase = new UploadMedias (this.videoRepository.Object, this.storageService.Object, this.unitOfWork.Object);

    }

    [Fact(DisplayName = nameof(UploadMedias))]
    [Trait("Application", "UplaodMedias - Use Cases")]
    public async Task UploadMedias()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var input = this.fixture.GetValidInput(exampleVideo.Id);
        var fileNames = new List<string>()
        {
            StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile.Extension),
            StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TrailerFile.Extension)
        };
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(x=> x == exampleVideo.Id), It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);

        this.storageService.Setup(x=> x.Upload(It.IsAny<string>(),It.IsAny<Stream>() ,It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());

         await this.useCase.Handle(input, CancellationToken.None);

        this.storageService.Verify(x=> x.Upload(It.Is<string>(x => fileNames.Contains(x)), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ThrowsWhenVideoNotFound))]
    [Trait("Application", "UploadMedias - Use Cases")]
    public async Task ThrowsWhenVideoNotFound()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var input = this.fixture.GetValidInput(exampleVideo.Id);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(x=> x == exampleVideo.Id), It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);

        this.storageService.Setup(x=> x.Upload(It.IsAny<string>(),It.IsAny<Stream>() ,It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        var action = async () => await this.useCase.Handle(input, CancellationToken.None);

       await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video not found");

    }

    [Fact(DisplayName = nameof(ClearStorageInUploadErrorCase))]
    [Trait("Application", "UplaodMedias - Use Cases")]
    public async Task ClearStorageInUploadErrorCase()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var input = this.fixture.GetValidInput(exampleVideo.Id);
        var videoFileName = StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile.Extension);

        var trailerFileName =
            StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TrailerFile.Extension);

        var videoStoragePath = $"storage/{videoFileName}";

        var fileNames = new List<string>()
        {
            videoFileName,
            trailerFileName
        };

        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(x=> x == exampleVideo.Id), It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);

        this.storageService.Setup(x=> x.Upload(It.Is<string>(x => x == videoFileName),It.IsAny<Stream>() ,It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoStoragePath);

        this.storageService.Setup(x=> x.Upload(It.Is<string>(x => x == trailerFileName),It.IsAny<Stream>() ,It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong with upload"));

        var action = async () => await this.useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>();

        this.videoRepository.VerifyAll();
        this.storageService.Verify(x=> x.Upload(It.Is<string>(x => fileNames.Contains(x)), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        this.storageService.Verify(x => x.Delete(It.Is<string>(x => x == videoStoragePath), It.IsAny<CancellationToken>()),
            Times.Exactly(1));

        this.storageService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(1));
    }

    [Fact(DisplayName = nameof(ClearStorageInCommitErrorCase))]
    [Trait("Application", "UplaodMedias - Use Cases")]
    public async Task ClearStorageInCommitErrorCase()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var input = this.fixture.GetValidInput(exampleVideo.Id);
        var videoFileName = StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile.Extension);

        var trailerFileName =
            StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Trailer), input.TrailerFile.Extension);

        var videoStoragePath = $"storage/{videoFileName}";
        var trailerStoragePath = $"storage/{trailerFileName}";

        var fileNames = new List<string>()
        {
            videoFileName,
            trailerFileName
        };

        var fileNamesPaths = new List<string>()
        {
            videoStoragePath,
            trailerStoragePath
        };

        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(x=> x == exampleVideo.Id), It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);

        this.storageService.Setup(x=> x.Upload(It.Is<string>(x => x == videoFileName),It.IsAny<Stream>() ,It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoStoragePath);

        this.storageService.Setup(x=> x.Upload(It.Is<string>(x => x == trailerFileName),It.IsAny<Stream>() ,It.IsAny<CancellationToken>()))
            .ReturnsAsync(trailerStoragePath);

        this.unitOfWork.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong with the commit"));


        var action = async () => await this.useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong with the commit");

        this.videoRepository.VerifyAll();

        this.storageService.Verify(x=> x.Upload(It.Is<string>(x => fileNames.Contains(x)), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        this.storageService.Verify(x => x.Delete(It.Is<string>(x => fileNamesPaths.Contains(x)), It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        this.storageService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(ClearOnlyOneFileInStorageInCommitErrorCaseIfProvidedOnlyOneFile))]
    [Trait("Application", "UplaodMedias - Use Cases")]
    public async Task ClearOnlyOneFileInStorageInCommitErrorCaseIfProvidedOnlyOneFile()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        exampleVideo.UpdateTrailer(this.fixture.GetValidMediaPath());
        exampleVideo.UpdateMedia(this.fixture.GetValidMediaPath());
        var input = this.fixture.GetValidInput(exampleVideo.Id, withTrailerFile: false);
        var videoFileName = StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Media), input.VideoFile.Extension);


        var videoStoragePath = $"storage/{videoFileName}";

        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(x=> x == exampleVideo.Id), It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);

        this.storageService.Setup(x=> x.Upload(It.Is<string>(x => x == videoFileName),It.IsAny<Stream>() ,It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoStoragePath);

        this.unitOfWork.Setup(x => x.Commit(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong with the commit"));


        var action = async () => await this.useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong with the commit");

        this.videoRepository.VerifyAll();

        this.storageService.Verify(x=> x.Upload(It.Is<string>(x => x == videoFileName), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

        this.storageService.Verify(x=> x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(1));

        this.storageService.Verify(x => x.Delete(It.Is<string>(x => x == videoStoragePath), It.IsAny<CancellationToken>()),
            Times.Exactly(1));

        this.storageService.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(1));
    }
}