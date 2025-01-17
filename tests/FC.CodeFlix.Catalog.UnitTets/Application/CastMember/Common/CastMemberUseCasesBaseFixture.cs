// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.CastMember.Common;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using UnitTets.Common;

public class CastMemberUseCasesBaseFixture : BaseFixture
{
    public string GetValidName() => this.Faker.Name.FullName();

    public CastMemberType GetRandomCastMemberType() => (CastMemberType)new Random().Next(1, 2);

    public CastMember GetExampleCastMember() => new(this.GetValidName(), this.GetRandomCastMemberType());
}