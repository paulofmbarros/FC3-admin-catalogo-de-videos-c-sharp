﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

using Common;
using MediatR;

public interface ICreateVideo : IRequestHandler<CreateVideoInput, VideoModelOutput>
{
    
}