// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Messaging.Extensions;

using Newtonsoft.Json.Serialization;

public static class StringSnakeCaseExtensions
{
    private static readonly NamingStrategy SnakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

    public static string ToSnakeCase(this string stringToConvert)
    {
        ArgumentNullException.ThrowIfNull(stringToConvert, nameof(stringToConvert));
        return SnakeCaseNamingStrategy.GetPropertyName(stringToConvert, false);
    }
}
