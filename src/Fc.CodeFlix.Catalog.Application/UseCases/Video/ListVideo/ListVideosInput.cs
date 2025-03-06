// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.ListVideo;

using Application.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;

public class ListVideosInput : PaginatedListInput, IRequest<ListVideosOutput>
{
    public ListVideosInput(int page, int perPage, string search, string sort, SearchOrder direction) : base(page, perPage, search, sort, direction)
    {
    }
}