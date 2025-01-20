// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CastMember.ListCastMembers;

using Common;
using Fc.CodeFlix.Catalog.Domain.Entity;

[CollectionDefinition(nameof(ListCastMembersTestFixture))]
public class ListCastMembersTestFixtureCollection : ICollectionFixture<ListCastMembersTestFixture>
{

}

public class ListCastMembersTestFixture : CastMemberUseCasesBaseFixture
{
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