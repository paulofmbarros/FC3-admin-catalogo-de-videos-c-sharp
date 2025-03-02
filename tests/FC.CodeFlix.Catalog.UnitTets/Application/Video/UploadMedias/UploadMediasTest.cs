// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.UploadMedias;

using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias;
using Fc.CodeFlix.Catalog.Domain.Repository;
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
        var input = this.fixture.GetValidInput();
        var exampleVideo = this.fixture.GetValidVideo();
        this.videoRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exampleVideo);

        this.storageService.Setup(x=> x.Upload(It.IsAny<string>(),It.IsAny<Stream>() ,It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid().ToString());

        this.useCase.Handle(input, CancellationToken.None);

        this.storageService.Verify(x=> x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}