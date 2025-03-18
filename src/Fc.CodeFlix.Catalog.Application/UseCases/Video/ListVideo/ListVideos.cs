// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.ListVideo;

using Common;
using Domain.Entity;
using Domain.Repository;
using Interfaces;

public class ListVideos : IListVideos
{
    private readonly IVideoRepository videoRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IGenreRepository genreRepository;

    public ListVideos(IVideoRepository videoRepository, ICategoryRepository categoryRepository, IGenreRepository genreRepository)
    {
        this.videoRepository = videoRepository;
        this.categoryRepository = categoryRepository;
        this.genreRepository = genreRepository;
    }

    public async Task<ListVideosOutput> Handle(ListVideosInput input, CancellationToken cancellationToken)
    {
        var searchResult = await this.videoRepository.Search(input.ToSearchInput(), cancellationToken);
        IReadOnlyList<Category> categories = null ;
        var relatedCategoriesIds = searchResult.Items.SelectMany(video =>video.Categories).Distinct().ToList();

        if (relatedCategoriesIds.Any())
        {
            categories = await this.categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
        }

        IReadOnlyList<Genre> genres = null ;
        var relatedGenresIds = searchResult.Items.SelectMany(video => video.Genres).Distinct().ToList();

        if (relatedGenresIds.Any())
        {
            genres = await this.genreRepository.GetListByIds(relatedGenresIds, cancellationToken);
        }


        var output =
         new ListVideosOutput(searchResult.CurrentPage,
             searchResult.PerPage,
             searchResult.Total,
             searchResult.Items.Select(item => VideoModelOutput.FromVideo(item, categories, genres))
                 .ToList());

        return output;

    }
}