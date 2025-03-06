// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.ListVideo;

using Common;
using Domain.Repository;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository videoRepository;

    public ListVideos(IVideoRepository videoRepository) => this.videoRepository = videoRepository;

    public async Task<ListVideosOutput> Handle(ListVideosInput input, CancellationToken cancellationToken)
    {
        var searchResult = await this.videoRepository.Search(input.ToSearchInput(), cancellationToken);

        return new ListVideosOutput(searchResult.CurrentPage, searchResult.PerPage, searchResult.Total, searchResult.Items.Select(VideoModelOutput.FromVideo).ToList());

    }
}