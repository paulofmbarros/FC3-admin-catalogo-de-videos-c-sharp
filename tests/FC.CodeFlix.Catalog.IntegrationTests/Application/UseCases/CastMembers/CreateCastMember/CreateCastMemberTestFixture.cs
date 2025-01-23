// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.CastMembers.CreateCastMember;

using Common;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberBaseFixtureCollection : ICollectionFixture<CreateCastMemberTestFixture>
{

}

public class CreateCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
}