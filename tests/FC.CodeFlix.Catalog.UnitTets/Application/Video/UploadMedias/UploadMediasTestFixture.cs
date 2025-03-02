// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.UploadMedias;

using Domain.Entity.Video;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Moq;
using UnitTets.Common.Fixtures;

[CollectionDefinition(nameof(UploadMediasTestFixture))]
public class UploadMediasTestFixtureCollection : ICollectionFixture<UploadMediasTestFixture>
{

}


public class UploadMediasTestFixture : VideoTestFixtureBase
{
    public UploadMediasInput GetValidInput(Guid? videoId = null,
        bool withVideoFile = true,
        bool withTrailerFile = true
        ) =>
        new (
            videoId?? Guid.NewGuid(),
            withVideoFile ? this.GetMediaValidFileInput() : null,
            withTrailerFile ? this.GetMediaValidFileInput() : null);

}