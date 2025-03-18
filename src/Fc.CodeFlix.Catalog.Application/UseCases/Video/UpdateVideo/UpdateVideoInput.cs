// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;

using Common;
using Domain.Enum;
using MediatR;

public record UpdateVideoInput
(
    Guid VideoId,
    string Title,
    string Description,
    int YearLaunched,
    bool Opened,
    bool Published,
    int Duration,
    Rating Rating,
    List<Guid>? GenreIds = null,
    List<Guid>? CategoryIds = null,
    List<Guid>? CastMemberIds = null,
    FileInput? Banner = null,
    FileInput? Thumb = null,
    FileInput? ThumbHalf = null
) : IRequest<VideoModelOutput>;