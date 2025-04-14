// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Infra.Storage.Service;

using Application.Interfaces;
using Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

public class StorageService : IStorageService
{

    private readonly StorageClient storageClient;
    private readonly StorageServiceOptions storageServiceOptions;

    public StorageService(StorageClient storageClient, IOptions<StorageServiceOptions> storageServiceOptions)
    {
        this.storageClient = storageClient;
        this.storageServiceOptions = storageServiceOptions.Value;
    }

    public async Task<string> Upload(string fileName, Stream fileStream, string contentType,
        CancellationToken cancellationToken)
    {
       await this.storageClient.UploadObjectAsync(this.storageServiceOptions.BucketName, fileName, contentType, fileStream, cancellationToken: cancellationToken);
       return fileName;
    }

    public async Task Delete(string filePath, CancellationToken cancellationToken)
    {
        await this.storageClient.DeleteObjectAsync(this.storageServiceOptions.BucketName, filePath, cancellationToken:cancellationToken);
    }
}