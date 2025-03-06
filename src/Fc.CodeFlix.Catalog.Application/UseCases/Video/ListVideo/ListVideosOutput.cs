// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.ListVideo;

using Application.Common;
using Common;

public class ListVideosOutput : PaginatedListOutput<VideoModelOutput>
{
    public ListVideosOutput(int page, int perPage, int total, IReadOnlyList<VideoModelOutput> items) : base(page, perPage, total, items)
    {
    }
}