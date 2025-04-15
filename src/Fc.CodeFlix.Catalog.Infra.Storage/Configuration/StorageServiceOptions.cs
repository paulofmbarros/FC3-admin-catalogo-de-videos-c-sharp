// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Infra.Storage.Configuration;

public class StorageServiceOptions
{
    public const string ConfigurationSection = "Storage";
    public string BucketName { get; set; }
}
