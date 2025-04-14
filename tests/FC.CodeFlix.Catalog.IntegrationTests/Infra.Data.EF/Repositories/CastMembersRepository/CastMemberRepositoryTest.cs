// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMembersRepository;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
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

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Update()
    {
        var castMemberExampleList = this.fixture.GetExampleCastMembersList(5);
        var exampleCastMember = castMemberExampleList[3];
        var newName = this.fixture.GetValidName();
        var newType = this.fixture.GetRandomCastMemberType();
        var context = this.fixture.CreateDbContext();
        await context.AddRangeAsync(castMemberExampleList);
        await context.SaveChangesAsync();
        exampleCastMember.Update(newName, newType);

        var actDbContext = this.fixture.CreateDbContext(true);
        var repository = new CastMemberRepository(actDbContext);
        await repository.Update(exampleCastMember, CancellationToken.None);
        await actDbContext.SaveChangesAsync();

        var assertDbContext = this.fixture.CreateDbContext(true);

        var castMembersFromDb =  await assertDbContext.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == exampleCastMember.Id);
        castMembersFromDb.Should().NotBeNull();
        castMembersFromDb.Name.Should().Be(newName);
        castMembersFromDb.Type.Should().Be(newType);

    }

    [Fact(DisplayName = nameof(Search))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Search()
    {
        var castMemberExampleList = this.fixture.GetExampleCastMembersList(10);
        var context = this.fixture.CreateDbContext();
        await context.AddRangeAsync(castMemberExampleList);
        await context.SaveChangesAsync();
        var repository = new CastMemberRepository(this.fixture.CreateDbContext(true));

        var searchInput = new SearchInput(1, 20, "","" , SearchOrder.Asc);
        var result = await repository.Search(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.Total.Should().Be(castMemberExampleList.Count);
        result.Items.Should().HaveCount(castMemberExampleList.Count);
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Items.ToList().ForEach(resultItem =>
        {
            var exampleCastMember = castMemberExampleList.FirstOrDefault(x => x.Id == resultItem.Id);
            exampleCastMember.Should().NotBeNull();
            resultItem.Name.Should().Be(exampleCastMember.Name);
            resultItem.Type.Should().Be(exampleCastMember.Type);

        });

    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenEmpty()
    {
        var repository = new CastMemberRepository(this.fixture.CreateDbContext());

        var searchInput = new SearchInput(1, 20, "","" , SearchOrder.Asc);
        var result = await repository.Search(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.Total.Should().Be(0);
        result.Items.Should().HaveCount(0);
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);

    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task SearchReturnsPaginated(int quantityCastMembersToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        CodeflixCatalogDbContext dbContext = this.fixture.CreateDbContext();
        var exampleCastMemberList = this.fixture.GetExampleCastMembersList(quantityCastMembersToGenerate);
        await dbContext.AddRangeAsync(exampleCastMemberList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchInput = new SearchInput(page,perPage,"","", SearchOrder.Asc);


        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(quantityCastMembersToGenerate);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData("Action",1,5,1,1)]
    [InlineData("Horror",1,5,3,3)]
    [InlineData("Horror",2,5,0,3)]
    [InlineData("Sci-Fi",1,5,4,4)]
    [InlineData("Sci-Fi",1,2,2,4)]
    [InlineData("Sci-Fi",2,3,1,4)]
    [InlineData("Sci-Fi other",1,3,0,0)]
    [InlineData("Robots",1,5,2,2)]
    public async Task SearchByText(string search ,int page, int perPage, int expectedQuantityItemsReturned, int expectedQuantityTotalItems)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCastMemberList = this.fixture.GetExampleCastMembersListByNames([
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-Fi IA",
            "Sci-Fi Space",
            "Sci-Fi Robots",
            "Sci-Fi Future",
        ]);

        await dbContext.AddRangeAsync(exampleCastMemberList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchInput = new SearchInput(page,perPage,search,"", SearchOrder.Asc);


        var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Items.ToList().ForEach(resultItem =>
        {
            var exampleCastMember = exampleCastMemberList.FirstOrDefault(x => x.Name == resultItem.Name);
            exampleCastMember.Should().NotBeNull();
            resultItem.Name.Should().Be(exampleCastMember.Name);
            resultItem.Type.Should().Be(exampleCastMember.Type);

        });

    }

    [Theory(DisplayName = nameof(OrderedSearch))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData("name","asc")]
    [InlineData("name","desc")]
    [InlineData("id","desc")]
    [InlineData("id","asc")]
    [InlineData("createdAt","asc")]
    [InlineData("createdAt","desc")]
    public async Task OrderedSearch(string orderBy, string order)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCastMemberList = this.fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleCastMemberList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new CastMemberRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1,20, "", orderBy, searchOrder);


        var output = await repository.Search(searchInput, CancellationToken.None);
        var expectedOrderedList = this.fixture.CloneCastMembersListOrdered(exampleCastMemberList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCastMemberList.Count);
        output.Items.Should().HaveCount(exampleCastMemberList.Count);
        output.Items.Should().BeEquivalentTo(expectedOrderedList, options => options.WithStrictOrdering());
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);


    }

    [Fact(DisplayName = nameof(GetIdsListByIds))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetIdsListByIds()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCastMembersList = this.fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = this.fixture.CreateDbContext(true);
        var repository = new CastMemberRepository(actDbContext);
        var idsToGet = exampleCastMembersList.Select(x => x.Id).Take(2).ToList();

        var result = await repository.GetIdsByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(idsToGet.Count);
        result.ToList().Should().BeEquivalentTo(idsToGet);

    }

    [Fact(DisplayName = nameof(GetIdsListByIdsWhenOnlyThreeIdsMatch))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetIdsListByIdsWhenOnlyThreeIdsMatch()
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleCastMembersList = this.fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleCastMembersList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = this.fixture.CreateDbContext(true);
        var repository = new CastMemberRepository(actDbContext);
        var idsToGet = exampleCastMembersList.Select(x => x.Id)
            .Take(3)
            .Concat(new []{Guid.NewGuid(), Guid.NewGuid(), })
            .ToList();

        var result = await repository.GetIdsByIds(idsToGet, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.ToList().Should().NotBeEquivalentTo(idsToGet);
        idsToGet.Should().Contain(result);

    }

}