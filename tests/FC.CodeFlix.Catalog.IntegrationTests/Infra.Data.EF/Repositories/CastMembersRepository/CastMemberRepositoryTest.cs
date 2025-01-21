// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMembersRepository;

using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTest
{
    private readonly CastMemberRepositoryTestFixture fixture;

    public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Insert()
    {
        var castMember = this.fixture.GetExampleCastMember();
        var context = this.fixture.CreateDbContext();
        var repository = new CastMemberRepository(context);
        await repository.Insert(castMember, CancellationToken.None);
        await context.SaveChangesAsync();

        var assertionContext = this.fixture.CreateDbContext(true);
        var castMemberFromDb = await assertionContext.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == castMember.Id);

        castMemberFromDb.Name.Should().Be(castMember.Name);
        castMemberFromDb.Type.Should().Be(castMember.Type);

    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Get()
    {
        var castMemberExampleList = this.fixture.GetExampleCastMembersList(5);
        var exampleCastMember = castMemberExampleList[3];
        var context = this.fixture.CreateDbContext();
        await context.AddRangeAsync(castMemberExampleList);
        await context.SaveChangesAsync();

        var repository = new CastMemberRepository(this.fixture.CreateDbContext(true));
        var castMemberFromDb = await repository.Get(exampleCastMember.Id, CancellationToken.None);

        castMemberFromDb.Should().NotBeNull();
        castMemberFromDb.Name.Should().Be(exampleCastMember.Name);
        castMemberFromDb.Type.Should().Be(exampleCastMember.Type);

    }

    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task GetThrowsWhenNotFound()
    {
        var randomGuid = Guid.NewGuid();

        var repository = new CastMemberRepository(this.fixture.CreateDbContext());
        var action = async () =>  await repository.Get(randomGuid, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember '{randomGuid}' not found.");

    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Delete()
    {
        var castMemberExampleList = this.fixture.GetExampleCastMembersList(5);
        var exampleCastMember = castMemberExampleList[3];
        var context = this.fixture.CreateDbContext();
        await context.AddRangeAsync(castMemberExampleList);
        await context.SaveChangesAsync();

        var repository = new CastMemberRepository(this.fixture.CreateDbContext(true));
        await repository.Delete(exampleCastMember, CancellationToken.None);

        var assertDbContext = this.fixture.CreateDbContext(true);

        var castMembersFromDb = assertDbContext.CastMembers.AsNoTracking().ToList();
        castMembersFromDb.Should().NotContain(exampleCastMember);


    }

}