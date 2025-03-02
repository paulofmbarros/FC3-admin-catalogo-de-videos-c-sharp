// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Common;

using Fc.CodeFlix.Catalog.Application.Common;
using FluentAssertions;

public class StorageFileNameTests
{
    [Fact()]
    [Trait("Application", "StorageName - Common")]
    public void CreateStorageNameForFile()
    {
        var exampleId = Guid.NewGuid();
        var exampleExtension = "mp4";
        var propertyName = "Video";
        var name = StorageFileName.Create(exampleId, propertyName, exampleExtension);

        name.Should().Be($"{exampleId}-{propertyName.ToLower()}.{exampleExtension}");

    }
}