// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.CreateVideo;

using System.Text;
using Domain.Entity.Video;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;
using UnitTets.Common.Fixtures;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture>
{

}


public class CreateVideoTestFixture : VideoTestFixtureBase
{
        public CreateVideoInput CreateValidInput(List<Guid>? categoriesIds = null,
                List<Guid>? genresIds = null,
                List<Guid>? castMembersIds = null,
                FileInput? thumb = null,
                FileInput? banner = null,
                FileInput? thumbHalf = null
                ) => new (
        this.GetValidTitle(),
        this.GetValidDescription(),
        this.GetValidYearLaunched(),
        this.GetRandomBoolean(),
        this.GetRandomBoolean(),
        this.GetValidDuration(),
        this.GetValidRating(),
        categoriesIds,
        genresIds,
        castMembersIds,
        thumb,
        banner,
        thumbHalf
        );

        public CreateVideoInput CreateValidInputWithAllData() => new (
                this.GetValidTitle(),
                this.GetValidDescription(),
                this.GetValidYearLaunched(),
                this.GetRandomBoolean(),
                this.GetRandomBoolean(),
                this.GetValidDuration(),
                this.GetValidRating(),
                null,
                null,
                null,
                this.GetImageValidFileInput(),
                this.GetImageValidFileInput(),
                this.GetImageValidFileInput()
        );
}