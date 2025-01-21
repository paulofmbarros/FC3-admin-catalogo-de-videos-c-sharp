// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMembersRepository;

using Base;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;

[CollectionDefinition(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTestFixtureCollection : ICollectionFixture<CastMemberRepositoryTestFixture>
{

}

public class CastMemberRepositoryTestFixture : BaseFixture
{
    public string GetValidName() => this.Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType() => (CastMemberType)new Random().Next(1, 2);

    public CastMember GetExampleCastMember() => new(this.GetValidName(), this.GetRandomCastMemberType());

    public List<CastMember> GetExampleCastMembersList(int quantity)
    {
        var castMembers = new List<CastMember>();
        for (var i = 0; i < quantity; i++)
        {
            castMembers.Add(new CastMember($"CastMember {i}", this.GetRandomCastMemberType()));
        }

        return castMembers;
    }
}