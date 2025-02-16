// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Entity;

using Enum;

public class Media
{
    public string FilePath { get; private set; }
    public string? EncodedPath { get; private set; }
    public MediaStatus Status { get; private set; }

    public Media(string filePath)
    {
        this.FilePath = filePath;
        this.Status = MediaStatus.Pending;
    }

    public void UpdateAsSentToEncode() => this.Status = MediaStatus.Processing;

    public void UpdateAsEncoded(string encodedPath)
    {
        this.Status = MediaStatus.Completed;
        this.EncodedPath = encodedPath;
    }
}