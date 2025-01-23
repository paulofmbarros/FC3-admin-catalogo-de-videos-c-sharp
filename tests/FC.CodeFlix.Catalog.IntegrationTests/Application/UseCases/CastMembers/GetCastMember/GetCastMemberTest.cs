// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.CastMembers.GetCastMember;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Common;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FluentAssertions;

[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class GetCastMemberTest(CastMemberUseCaseBaseFixture fixture)
{
    [Fact(DisplayName = nameof(GetCastMember))]
    [Trait("Integration/Application", "GetCastMember - Use Case ")]
    public async Task GetCastMember()
    {
        var arrangeDbContext = fixture.CreateDbContext();
        var castMembers = fixture.GetExampleCastMembersList(10);
        await arrangeDbContext.CastMembers.AddRangeAsync(castMembers);
        await arrangeDbContext.SaveChangesAsync();

        var castMember = castMembers.First();
        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var useCase = new GetCastMember(castMemberRepository);
        var input = new GetCastMemberInput(castMember.Id);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(castMember.Id);
        output.Name.Should().Be(castMember.Name);
        output.Type.Should().Be(castMember.Type);
        output.CreatedAt.Should().NotBe(default);
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Integration/Application", "GetCastMember - Use Case ")]
    public async Task ThrowWhenNotFound()
    {

        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var useCase = new GetCastMember(castMemberRepository);
        var randomGuid = Guid.NewGuid();
        var input = new GetCastMemberInput(randomGuid);

        var act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>($"CastMember '{randomGuid}' not found.");

    }

}