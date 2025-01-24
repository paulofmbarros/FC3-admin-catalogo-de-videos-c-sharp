// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CastMembers.Common;

using Base;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Xunit;

[CollectionDefinition(nameof(CastMemberApiBaseFixture))]
public class CastMemberApiBaseFixtureCollection : ICollectionFixture<CastMemberApiBaseFixture>
{
}

public class CastMemberApiBaseFixture : BaseFixture
{
    public CastMemberPersistence CastMemberPersistence { get; }

    public CastMemberApiBaseFixture()
    {
        this.CastMemberPersistence = new CastMemberPersistence(this.CreateDbContext());
    }

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

    public List<CastMember> GetExampleCastMembersListByNames(List<string> names)=>
        names.Select(name => new CastMember(name, this.GetRandomCastMemberType())).ToList();

    public List<CastMember> CloneCastMembersListOrdered(List<CastMember> genres,string orderBy, SearchOrder order)
    {
        var listCloned = new List<CastMember>(genres);
        var orderedList = (orderBy.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => listCloned.OrderBy(x => x.Name).ThenBy(x=>x.Id),
            ("name", SearchOrder.Desc) => listCloned.OrderByDescending(x => x.Name).ThenByDescending(x=>x.Id),
            ("id", SearchOrder.Asc) => listCloned.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listCloned.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listCloned.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listCloned.OrderByDescending(x => x.CreatedAt),
            _ => listCloned.OrderBy(x => x.Name).ThenBy(x=>x.Id)
        };

        return orderedList.ToList();
    }
}