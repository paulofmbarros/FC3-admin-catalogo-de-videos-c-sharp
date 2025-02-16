// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.CreateVideo;

using Common.Fixtures;
using Domain.Entity.Video;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>
{

}


public class CreateVideoTestFixture : VideoTestFixtureBase
{
        public CreateVideoInput CreateValidCreateVideoInput() => new CreateVideoInput(
        string.Empty,
        this.GetValidDescription(),
        this.GetValidYearLaunched(),
        this.GetRandomBoolean(),
        this.GetRandomBoolean(),
        this.GetValidDuration(),
        this.GetValidRating());
}