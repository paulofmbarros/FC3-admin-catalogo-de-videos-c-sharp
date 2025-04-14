// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.CastMembers.DeleteCastMember;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Common;
using Fc.CodeFlix.Catalog.Application;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class DeleteCastMemberTest(CastMemberUseCaseBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Application", "GetCastMember - Use Case ")]
    public async Task Delete()
    {
        var arrangeDbContext = fixture.CreateDbContext();
        var castMembers = fixture.GetExampleCastMembersList(10);
        await arrangeDbContext.CastMembers.AddRangeAsync(castMembers);
        await arrangeDbContext.SaveChangesAsync();

        var castMember = castMembers.First();
        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());
        var useCase = new DeleteCastMember(castMemberRepository,unitOfWork);
        var input = new DeleteCastMemberInput(castMember.Id);

        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = fixture.CreateDbContext(true);
        var castMemberDb = await assertDbContext.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == castMember.Id);

        castMemberDb.Should().BeNull();

    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Integration/Application", "GetCastMember - Use Case ")]
    public async Task ThrowWhenNotFound()
    {
        var actDbContext = fixture.CreateDbContext(true);
        var randomGuid = Guid.NewGuid();
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWork(actDbContext, eventPublisher, serviceProvider.GetService<ILogger<UnitOfWork>>());
        var useCase = new DeleteCastMember(castMemberRepository,unitOfWork);
        var input = new DeleteCastMemberInput(randomGuid);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>($"CastMember '{randomGuid}' not found.");

    }
}
