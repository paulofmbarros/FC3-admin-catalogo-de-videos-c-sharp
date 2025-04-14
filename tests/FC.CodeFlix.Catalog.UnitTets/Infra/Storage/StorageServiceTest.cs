// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Infra.Storage;

using System.Text;
using Fc.CodeFlix.Catalog.Infra.Storage.Configuration;
using Fc.CodeFlix.Catalog.Infra.Storage.Service;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Moq;
using GpcData = Google.Apis.Storage.v1.Data;
using Microsoft.Extensions.Options;

[Collection(nameof(StorageServiceTestFixture))]
public class StorageServiceTest
{
    private readonly StorageServiceTestFixture fixture;

    public StorageServiceTest(StorageServiceTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(Upload))]
    [Trait("Infrastructure.Storage" , "StorageService")]
    public async Task Upload()
    {
        var storageClientMock = new Mock<StorageClient>();
        var objectMock = new Mock<GpcData.Object>();

        storageClientMock.Setup(x => x.UploadObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Stream>(), It.IsAny<UploadObjectOptions>(), It.IsAny<CancellationToken>(), It.IsAny<IProgress<IUploadProgress>>()))
            .ReturnsAsync(objectMock.Object);

        var storageOptions = new StorageServiceOptions
        {
            BucketName = fixture.GetBucketName(),
        };
        var options = Options.Create<StorageServiceOptions>(storageOptions);
        var service = new StorageService(storageClientMock.Object, options);
        var fileName =this.fixture.GetFileName();
        var contentStream = Encoding.UTF8.GetBytes(fixture.GetContentFile());
        var stream = new MemoryStream(contentStream);
        var contentType = fixture.GetContentType();

        var filePath = await service.Upload(fileName,stream,contentType,CancellationToken.None);

        Assert.Equal(fileName, filePath);
        storageClientMock.Verify(x => x.UploadObjectAsync(storageOptions.BucketName, fileName, contentType,
                stream, It.IsAny<UploadObjectOptions>(), It.IsAny<CancellationToken>(), It.IsAny<IProgress<IUploadProgress>>()),
            Times.Once);

    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Infrastructure.Storage" , "StorageService")]
    public async Task Delete()
    {
        var storageClientMock = new Mock<StorageClient>();

        storageClientMock.Setup(x => x.DeleteObjectAsync(It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<DeleteObjectOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var storageOptions = new StorageServiceOptions
        {
            BucketName = fixture.GetBucketName(),
        };
        var options = Options.Create(storageOptions);
        var service = new StorageService(storageClientMock.Object, options);
        var fileName =this.fixture.GetFileName();

        await service.Delete(fileName, CancellationToken.None);

        storageClientMock.Verify(x => x.DeleteObjectAsync(storageOptions.BucketName, fileName,
            It.IsAny<DeleteObjectOptions>(), It.IsAny<CancellationToken>()), Times.Once);

    }


    
}