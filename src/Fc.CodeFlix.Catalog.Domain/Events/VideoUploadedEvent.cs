// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Events;

using SeedWork;

public class VideoUploadedEvent : DomainEvent
{
    public Guid ResourceId { get; set; }
    public string FilePath { get; set; }

    public VideoUploadedEvent(Guid resourceId, string filePath) : base()
    {
        this.ResourceId = resourceId;
        this.FilePath = filePath;
    }
}