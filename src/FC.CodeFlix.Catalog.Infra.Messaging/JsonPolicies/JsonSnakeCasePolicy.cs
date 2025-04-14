// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Messaging.JsonPolicies;

using System.Text.Json;
using Extensions;

public class JsonSnakeCasePolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToSnakeCase();
}
