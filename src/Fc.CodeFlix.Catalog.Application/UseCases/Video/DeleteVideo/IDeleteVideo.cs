// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.DeleteVideo;

using MediatR;

public interface IDeleteVideo : IRequestHandler<DeleteVideoInput, Unit>;