// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.ListVideos;

using Domain.Entity.Video;
using Fc.CodeFlix.Catalog.Domain.Entity;

[CollectionDefinition(nameof(ListVideosTestFixture))]
public class ListVideosTestFixtureCollection : ICollectionFixture<ListVideosTestFixture>
{

}


public class ListVideosTestFixture : VideoTestFixture
{
    public List<Video> CreateExampleVideosList()
       => Enumerable.Range(1, Random.Shared.Next(2,10)).Select(_ => this.GetValidVideoWithAllProperties()).ToList();

}