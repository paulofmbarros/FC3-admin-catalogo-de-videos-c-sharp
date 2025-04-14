// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Messaging.Configuration;

using Fc.CodeFlix.Catalog.Domain.Events;

public static class EventsMapping
{
    private static Dictionary<string, string> RoutingKeys => new()
    {
        {typeof(VideoUploadedEvent).Name, "video.created"},
    };

    public static string GetRoutingKey<T>() => RoutingKeys[typeof(T).Name];
}
