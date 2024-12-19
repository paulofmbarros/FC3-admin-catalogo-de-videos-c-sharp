// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.GetGenre;

using Common;
using Xunit;

[CollectionDefinition(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTestFixtureCollection : ICollectionFixture<GetGenreApiTestFixture>
{
}

public class GetGenreApiTestFixture : GenreBaseFixture
{
    public GetGenreApiTestFixture() : base()
    {

    }
}