// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.GetVideo;

using Common;
using MediatR;

public record GetVideoInput(Guid VideoId) : IRequest<VideoModelOutput>;