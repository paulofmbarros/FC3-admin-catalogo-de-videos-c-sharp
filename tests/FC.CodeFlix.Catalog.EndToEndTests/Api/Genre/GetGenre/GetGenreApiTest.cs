// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.GetGenre;

using Xunit;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest(GetGenreApiTestFixture fixture)
{
    private readonly GetGenreApiTestFixture fixture = fixture;
}