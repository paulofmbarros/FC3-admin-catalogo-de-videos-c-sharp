// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Interfaces;

public interface IStorageService
{
    Task<string> Upload(string fileName, Stream fileStream, string contentType,  CancellationToken cancellationToken);
    Task Delete(string filePath, CancellationToken cancellationToken);
}