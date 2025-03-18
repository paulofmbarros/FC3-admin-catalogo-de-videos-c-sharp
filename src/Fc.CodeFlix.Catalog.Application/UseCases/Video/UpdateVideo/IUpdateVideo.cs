// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;

using Common;
using MediatR;

public interface IUpdateVideo : IRequestHandler<UpdateVideoInput, VideoModelOutput>
{
    
}