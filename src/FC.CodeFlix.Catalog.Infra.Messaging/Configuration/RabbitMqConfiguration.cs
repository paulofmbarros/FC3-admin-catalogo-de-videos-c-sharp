// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Messaging.Configuration;

public class RabbitMqConfiguration
{
    public const string ConfigurationSection = "RabbitMq";
    public string? Hostname { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Exchange { get; set; }
}
