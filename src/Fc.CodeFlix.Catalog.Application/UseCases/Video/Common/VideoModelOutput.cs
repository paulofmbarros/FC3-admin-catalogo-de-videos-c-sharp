// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.Common;

using Domain.Entity;
using Domain.Enum;
using Domain.Extensions;
using Domain.SeedWork;

public record VideoModelOutput(
    Guid Id,
    DateTime CreatedAt,
    string Title,
    bool Published,
    string Description,
    string Rating,
    int YearLaunched,
    bool Opened,
    int Duration,
    IReadOnlyCollection<VideoModelOutput.VideoModelOutputRelatedAggregate> Categories,
    IReadOnlyCollection<VideoModelOutput.VideoModelOutputRelatedAggregate> Genres,
    IReadOnlyCollection<VideoModelOutput.VideoModelOutputRelatedAggregate> CastMembers,
    string? ThumbFileUrl,
    string? BannerFileUrl,
    string? ThumbHalf,
    string? MediaFileUrl,
    string? TrailerFileUrl
)
{
    public static VideoModelOutput FromVideo(Video video) => new (
        video.Id,
        video.CreatedAt,
        video.Title,
        video.Published,
        video.Description,
        video.Rating.ToStringSignal(),
        video.YearLaunched,
        video.Opened,
        video.Duration,
        video.Categories.Select(id =>  new VideoModelOutputRelatedAggregate(id)).ToList(),
        video.Genres.Select(id =>  new VideoModelOutputRelatedAggregate(id)).ToList(),
        video.CastMembers.Select(id =>  new VideoModelOutputRelatedAggregate(id)).ToList(),
        video.Thumb?.Path,
        video.Banner?.Path,
        video.ThumbHalf?.Path,
        video.Media?.FilePath,
        video.Trailer?.FilePath
    );

    public static VideoModelOutput FromVideo(Video video, IReadOnlyCollection<Category> categories , IReadOnlyCollection<Genre> genres) => new (
        video.Id,
        video.CreatedAt,
        video.Title,
        video.Published,
        video.Description,
        video.Rating.ToStringSignal(),
        video.YearLaunched,
        video.Opened,
        video.Duration,
        video.Categories.Select(id =>
            new VideoModelOutputRelatedAggregate(id, categories?.FirstOrDefault(category=> category.Id == id)?.Name))
            .ToList(),
        video.Genres.Select(id =>
                new VideoModelOutputRelatedAggregate(id, genres?.FirstOrDefault(genre=> genre.Id == id)?.Name))
            .ToList(),
        video.CastMembers.Select(id =>  new VideoModelOutputRelatedAggregate(id)).ToList(),
        video.Thumb?.Path,
        video.Banner?.Path,
        video.ThumbHalf?.Path,
        video.Media?.FilePath,
        video.Trailer?.FilePath
    );



    public record VideoModelOutputRelatedAggregate(Guid Id, string? Name = null);

}