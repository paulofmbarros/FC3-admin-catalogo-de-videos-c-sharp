// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.DeleteVideo;

using Fc.CodeFlix.Catalog.Application.UseCases.Video.DeleteVideo;
using UnitTets.Common.Fixtures;

[CollectionDefinition(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTestFixtureCollection : ICollectionFixture<DeleteVideoTestFixture>
{

}


public class DeleteVideoTestFixture : VideoTestFixtureBase
{
    public DeleteVideoInput GetValidInput(Guid? id = null) => new (id ?? Guid.NewGuid());
}