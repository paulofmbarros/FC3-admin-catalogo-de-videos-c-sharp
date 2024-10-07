// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Configurations.Policies;

using System.Text.Json;
using Extensions.String;

public class JsonSnakeCasePolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToSnakeCase();
}