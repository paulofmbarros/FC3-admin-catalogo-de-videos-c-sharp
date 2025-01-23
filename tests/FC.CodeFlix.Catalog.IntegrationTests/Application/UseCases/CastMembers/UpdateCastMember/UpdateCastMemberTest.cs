// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.CastMembers.UpdateCastMember;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Common;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using Fc.CodeFlix.Catalog.Domain.Enum;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class UpdateCastMemberTest(CastMemberUseCaseBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Application", "UpdateCastMember - Use Case ")]
    public async Task Update()
    {
        var examples = fixture.GetExampleCastMembersList(10);
        var castMember = examples.First();
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.CastMembers.AddRangeAsync(examples);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var unitOfWork = new UnitOfWork(actDbContext);

        var useCase = new UpdateCastMember(unitOfWork, castMemberRepository);

        var input = new UpdateCastMemberInput(castMember.Id, fixture.GetValidName(), fixture.GetRandomCastMemberType());

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(castMember.Id);
        output.Name.Should().Be(input.Name);
        output.Type.Should().Be(input.Type);

        var assertDbContext = fixture.CreateDbContext(true);
        var castMemberDb = await assertDbContext.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == output.Id);

        castMemberDb.Should().NotBeNull();
        castMemberDb.Name.Should().Be(input.Name);
        castMemberDb.Type.Should().Be(input.Type);

    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Integration/Application", "UpdateCastMember - Use Case ")]
    public async Task ThrowWhenNotFound()
    {
        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var unitOfWork = new UnitOfWork(actDbContext);
        var randomGuid = Guid.NewGuid();

        var useCase = new UpdateCastMember(unitOfWork, castMemberRepository);

        var input = new UpdateCastMemberInput(randomGuid, fixture.GetValidName(), fixture.GetRandomCastMemberType());

        var result = async () => await useCase.Handle(input, CancellationToken.None);

        await result.Should().ThrowAsync<NotFoundException>($"CastMember '{randomGuid}' not found.");

    }
}