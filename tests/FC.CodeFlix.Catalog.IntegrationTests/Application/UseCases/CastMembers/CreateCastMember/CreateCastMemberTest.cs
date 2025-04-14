// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.CastMembers.CreateCastMember;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest(CreateCastMemberTestFixture fixture)
{
    [Fact(DisplayName = nameof(CreateCastMember))]
    [Trait("Integration/Application", "CreateCategory - Use Case ")]
    public async Task CreateCastMember()
    {
        var actDbContext = fixture.CreateDbContext();
        var castMember = fixture.GetExampleCastMember();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var useCase = new CreateCastMember(castMemberRepository, unitOfWork);
        var input = new CreateCastMemberInput(castMember.Name, castMember.Type);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(castMember.Name);
        output.Type.Should().Be(castMember.Type);
        output.CreatedAt.Should().NotBe(default);

        var assertDbContext = fixture.CreateDbContext(true);
        var castMemberDb = await assertDbContext.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == output.Id);

        castMemberDb.Should().NotBeNull();
        castMemberDb.Name.Should().Be(castMember.Name);
        castMemberDb.Type.Should().Be(castMember.Type);
        castMemberDb.CreatedAt.Should().NotBe(default);

    }

}
