// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.GetVideo;

using Common;
using Domain.Repository;

public class GetVideo : IGetVideo
{
    private readonly IVideoRepository videoRepository;

    public GetVideo(IVideoRepository videoRepository) => this.videoRepository = videoRepository;

    public async Task<VideoModelOutput> Handle(GetVideoInput input, CancellationToken cancellationToken)
    {
        var video = await this.videoRepository.Get(input.VideoId, cancellationToken);
        return VideoModelOutput.FromVideo(video);
    }
}