// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.Common;

using Domain.Entity;
using Domain.Enum;

public record VideoModelOutput(
    Guid Id,
    DateTime CreatedAt,
    string Title,
    bool Published,
    string Description,
    Rating Rating,
    int YearLaunched,
    bool Opened,
    int Duration,
    IReadOnlyCollection<Guid> CategoriesIds,
    IReadOnlyCollection<Guid> GenresIds,
    IReadOnlyCollection<Guid> CastMembersIds,
    string? Thumb,
    string? Banner,
    string? ThumbHalf,
    string? Media,
    string? Trailer
)
{
    public static VideoModelOutput FromVideo(Video video) => new (
        video.Id,
        video.CreatedAt,
        video.Title,
        video.Published,
        video.Description,
        video.Rating,
        video.YearLaunched,
        video.Opened,
        video.Duration,
        video.Categories,
        video.Genres,
        video.CastMembers,
        video.Thumb?.Path,
        video.Banner?.Path,
        video.ThumbHalf?.Path,
        video.Media?.FilePath,
        video.Trailer?.FilePath
    );
}