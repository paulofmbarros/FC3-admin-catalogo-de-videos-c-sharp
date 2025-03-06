// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.ListVideo;

using MediatR;

public interface IListVideos : IRequestHandler<ListVideosInput, ListVideosOutput>
{
    
}