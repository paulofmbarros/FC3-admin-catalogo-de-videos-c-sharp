// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Common;

public static class StorageFileName
{
    public static string Create(Guid id, string propertyName, string extension) =>
        $"{id}-{propertyName.ToLower()}.{extension}";
}