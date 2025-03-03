// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.DeleteVideo;

using Domain.Repository;
using Interfaces;
using MediatR;

public class DeleteVideo : IDeleteVideo
{
    private readonly IVideoRepository videoRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IStorageService storageService;

    public DeleteVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork, IStorageService storageService)
    {
        this.videoRepository = videoRepository;
        this.unitOfWork = unitOfWork;
        this.storageService = storageService;
    }

    public async Task<Unit> Handle(DeleteVideoInput request, CancellationToken cancellationToken)
    {
        var video = await this.videoRepository.Get(request.VideoId, cancellationToken);
        await this.videoRepository.Delete(video, cancellationToken);

        await this.unitOfWork.Commit(cancellationToken);

        if (video.Trailer is not null)
        {
           await this.storageService.Delete(video.Trailer.FilePath, cancellationToken);
        }

        if (video.Media is not null)
        {
            await this.storageService.Delete(video.Media.FilePath, cancellationToken);
        }

        return Unit.Value;
    }
}