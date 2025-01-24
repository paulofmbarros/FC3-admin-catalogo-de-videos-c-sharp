// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.CastMembers.ListCastMembers;

using Catalog.Infra.Data.EF.Repositories;
using Common;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;

[Collection(nameof(CastMemberUseCaseBaseFixture))]
public class ListCastMembersTest(CastMemberUseCaseBaseFixture fixture)
{
    [Fact(DisplayName = nameof(SimpleList))]
    [Trait("Integration/Application", "ListCastMembers - Use Case ")]
    public async Task SimpleList()
    {
        var examplesList = fixture.GetExampleCastMembersList(10);
        var arrangeDbContext = fixture.CreateDbContext();
        await arrangeDbContext.CastMembers.AddRangeAsync(examplesList);
        await arrangeDbContext.SaveChangesAsync();

        var actDbContext = fixture.CreateDbContext(true);
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var useCase = new ListCastMembers(castMemberRepository);
        var input = new ListCastMembersInput(1,10,"", "", SearchOrder.Asc);

        var output = await useCase.Handle(input, CancellationToken.None);
        output.Should().NotBeNull();
        output.Items.Should().NotBeEmpty();
        output.Items.Count.Should().Be(input.PerPage);
        output.Total.Should().Be(examplesList.Count);
        output.PerPage = input.PerPage;
        output.Page = input.Page;
        output.Items.ToList().ForEach(item =>
        {
           var exampleItem = examplesList.FirstOrDefault(x => x.Id == item.Id);
              exampleItem.Should().NotBeNull();
              exampleItem.Should().BeEquivalentTo(item);

        });
    }

    [Fact(DisplayName = nameof(Empty))]
    [Trait("Integration/Application", "ListCastMembers - Use Case ")]
    public async Task Empty()
    {
        var actDbContext = fixture.CreateDbContext();
        var castMemberRepository = new CastMemberRepository(actDbContext);
        var useCase = new ListCastMembers(castMemberRepository);
        var input = new ListCastMembersInput(1,10,"", "", SearchOrder.Asc);

        var output = await useCase.Handle(input, CancellationToken.None);
        output.Should().NotBeNull();
        output.Items.Should().BeEmpty();
        output.Items.Count.Should().Be(0);
        output.Total.Should().Be(0);
        output.PerPage = input.PerPage;
        output.Page = input.Page;
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [Trait("Integration/Application", "ListCastMembers - Use Case ")]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    public async Task SearchReturnsPaginated(int quantityCastMembersToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        var dbContext = fixture.CreateDbContext();
        var exampleCastMemberList = fixture.GetExampleCastMembersList(quantityCastMembersToGenerate);
        await dbContext.AddRangeAsync(exampleCastMemberList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var castMemberRepository = new CastMemberRepository(dbContext);
        var searchInput = new ListCastMembersInput(page,perPage,"","", SearchOrder.Asc);
        var useCase = new ListCastMembers(castMemberRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(quantityCastMembersToGenerate);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Page.Should().Be(searchInput.Page);
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
        var dbContext = fixture.CreateDbContext();
        var exampleCastMemberList = fixture.GetExampleCastMembersListByNames([
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
        var searchInput = new ListCastMembersInput(page,perPage,search,"", SearchOrder.Asc);
        var useCase = new ListCastMembers(castMemberRepository);


        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Page.Should().Be(searchInput.Page);
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
        var dbContext = fixture.CreateDbContext();
        var exampleCastMemberList = fixture.GetExampleCastMembersList(10);
        await dbContext.AddRangeAsync(exampleCastMemberList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new CastMemberRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new ListCastMembersInput(1,20, "", orderBy, searchOrder);
        var useCase = new ListCastMembers(repository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);
        var expectedOrderedList = fixture.CloneCastMembersListOrdered(exampleCastMemberList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleCastMemberList.Count);
        output.Items.Should().HaveCount(exampleCastMemberList.Count);
        output.Items.Should().BeEquivalentTo(expectedOrderedList, options => options.WithStrictOrdering());
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);


    }
}