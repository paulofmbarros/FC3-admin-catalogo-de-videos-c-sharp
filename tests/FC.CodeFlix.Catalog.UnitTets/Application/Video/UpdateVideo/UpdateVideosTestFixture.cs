// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.UpdateVideo;

using Fc.CodeFlix.Catalog.Application.UseCases.Video.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;
using UnitTets.Common.Fixtures;

[CollectionDefinition(nameof(UpdateVideosTestFixture))]
public class UpdateVideosTestFixtureCollection : ICollectionFixture<UpdateVideosTestFixture>
{

}


public class UpdateVideosTestFixture : VideoTestFixtureBase
{
    public UpdateVideoInput CreateValidInput(Guid videoId,  List<Guid>? genresIds = null, List<Guid>? categoryIds = null, List<Guid>? castMemberIds = null, FileInput? banner = null, FileInput? thumb = null, FileInput? thumbHalf = null) =>
        new UpdateVideoInput(
            videoId,
            this.GetValidTitle(),
            this.GetValidDescription(),
            this.GetValidYearLaunched(),
            this.GetRandomBoolean(),
            this.GetRandomBoolean(),
            this.GetValidDuration(),
            this.GetValidRating(),
            genresIds,
            categoryIds,
            castMemberIds,
            banner,
            thumb,
            thumbHalf);
}