// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias;

using Application.Common;
using Domain.Entity;
using Domain.Repository;
using Interfaces;
using MediatR;

public class UploadMedias : IUploadMedias
{
    private readonly IVideoRepository videoRepository;
    private readonly IStorageService storageService;
    private readonly IUnitOfWork unitOfWork;

    public UploadMedias(IVideoRepository videoRepository, IStorageService storageService, IUnitOfWork unitOfWork)
    {
        this.videoRepository = videoRepository;
        this.storageService = storageService;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UploadMediasInput input, CancellationToken cancellationToken)
    {
        var video = await this.videoRepository.Get(input.VideoId, cancellationToken);

        try
        {
            await this.UploadVideo(input, cancellationToken, video);
            await this.UploadTrailer(input, cancellationToken, video);

            await this.videoRepository.Update(video, cancellationToken);
            await this.unitOfWork.Commit(cancellationToken);
            return Unit.Value;
        }
        catch (Exception e)
        {
            await this.ClearStorage(input, cancellationToken, video);
            throw;
        }
    }

    private async Task ClearStorage(UploadMediasInput input, CancellationToken cancellationToken, Video video)
    {
        if (input.VideoFile is not null && video.Media is not null)
        {
            await this.storageService.Delete(video.Media.FilePath, cancellationToken);
        }

        if (input.TrailerFile is not null && video.Trailer is not null)
        {
            await this.storageService.Delete(video.Trailer.FilePath, cancellationToken);
        }
    }

    private async Task UploadTrailer(UploadMediasInput input, CancellationToken cancellationToken, Video video)
    {
        if (input.TrailerFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.TrailerFile.Extension);
            var uploadedFilePath = await this.storageService.Upload(fileName, input.TrailerFile.FileStream,  cancellationToken);
            video.UpdateTrailer(uploadedFilePath);
        }
    }

    private async Task UploadVideo(UploadMediasInput input, CancellationToken cancellationToken, Video video)
    {
        if (input.VideoFile is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), input.VideoFile.Extension);
            var uploadedFilePath = await this.storageService.Upload(fileName, input.VideoFile.FileStream,  cancellationToken);
            video.UpdateMedia(uploadedFilePath);
        }
    }
}