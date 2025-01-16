// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.CastMember;

using Common;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;

[CollectionDefinition(nameof(CastMemberFixture))]
public class CastMemberFixtureCollection : ICollectionFixture<CastMemberFixture>
{
}

public class CastMemberFixture : BaseFixture
{
    public string GetValidName() => this.Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType() => (CastMemberType)new Random().Next(1, 2);

    public CastMember GetExampleCastMember() => new(this.GetValidName(), this.GetRandomCastMemberType());
}