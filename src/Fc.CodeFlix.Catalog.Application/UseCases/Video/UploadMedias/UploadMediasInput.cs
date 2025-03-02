// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias;

using Common;
using MediatR;

public record UploadMediasInput(Guid VideoId, FileInput? VideoFile, FileInput? TrailerFile) : IRequest<Unit>;