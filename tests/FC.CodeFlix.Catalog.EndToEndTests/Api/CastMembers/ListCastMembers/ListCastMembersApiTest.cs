// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CastMembers.ListCastMembers;

using Common;
using Extensions.DateTime;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Models;
using Xunit;

[Collection(nameof(CastMemberApiBaseFixture))]
public class ListCastMembersApiTest : IDisposable
{
    private readonly CastMemberApiBaseFixture fixture;

    public ListCastMembersApiTest(CastMemberApiBaseFixture fixture)
    {
        this.fixture = fixture;
        this.fixture.ClearPersistence();
    }

    [Fact(DisplayName = nameof(List))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task List()
    {
        var examples = fixture.GetExampleCastMembersList(5);
        await fixture.CastMemberPersistence.InsertList(examples);

        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("castMembers");

        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(examples.Count);
        output.Data.ToList().ForEach(item =>
        {
            var expectedOutput = examples.FirstOrDefault(x => x.Id == item.Id);
            expectedOutput.Should().NotBeNull();
            item.Name.Should().Be(expectedOutput.Name);
            item.Type.Should().Be(expectedOutput.Type);
        });
    }

    [Fact(DisplayName = nameof(ReturnsEmptyWhenEmpty))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task ReturnsEmptyWhenEmpty()
    {
        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("castMembers");

        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(0);
        output.Meta.Total.Should().Be(0);
    }

    [Theory(DisplayName = nameof(Paginated))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task Paginated(int quantityCastMembersToGenerate, int page, int perPage, int expectedQuantityItems)
    {
        var examples = fixture.GetExampleCastMembersList(quantityCastMembersToGenerate);
        await fixture.CastMemberPersistence.InsertList(examples);

        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("castMembers",
                new ListCastMembersInput(page, perPage, "", "", SearchOrder.Asc));

        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItems);
        output.Data.ToList().ForEach(item =>
        {
            var expectedOutput = examples.FirstOrDefault(x => x.Id == item.Id);
            expectedOutput.Should().NotBeNull();
            item.Name.Should().Be(expectedOutput.Name);
            item.Type.Should().Be(expectedOutput.Type);
        });
        output.Meta.CurrentPage.Should().Be(page);
        output.Meta.PerPage.Should().Be(perPage);
        output.Meta.Total.Should().Be(examples.Count);
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
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
        var examples = fixture.GetExampleCastMembersListByNames([
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
        await fixture.CastMemberPersistence.InsertList(examples);

        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("castMembers",
                new ListCastMembersInput(page, perPage, search, "", SearchOrder.Asc));

        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);
        output.Data.ToList().ForEach(item =>
        {
            var expectedOutput = examples.FirstOrDefault(x => x.Id == item.Id);
            expectedOutput.Should().NotBeNull();
            item.Name.Should().Be(expectedOutput.Name);
            item.Type.Should().Be(expectedOutput.Type);
        });
        output.Meta.CurrentPage.Should().Be(page);
        output.Meta.PerPage.Should().Be(perPage);
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
    }

    [Theory(DisplayName = nameof(Ordering))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "desc")]
    [InlineData("id", "asc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task Ordering(string orderBy, string order)
    {
        var examples = fixture.GetExampleCastMembersList(10);
        await fixture.CastMemberPersistence.InsertList(examples);
        var searchOrder = order.Equals("asc", StringComparison.CurrentCultureIgnoreCase) ? SearchOrder.Asc : SearchOrder.Desc;


        var input = new ListCastMembersInput(1, 20 , "", orderBy, searchOrder);

        var (response, output) =
            await this.fixture.ApiClient.Get<TestApiResponseList<CastMemberModelOutput>>("castMembers",
                input);



        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Should().HaveCount(examples.Count);
        output.Data.ToList().ForEach(item =>
        {
            var expectedOutput = examples.FirstOrDefault(x => x.Id == item.Id);
            expectedOutput.Should().NotBeNull();
            item.Name.Should().Be(expectedOutput.Name);
            item.Type.Should().Be(expectedOutput.Type);
        });
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.Total.Should().Be(examples.Count);

        var orderedList =
            fixture.CloneCastMembersListOrdered(examples, orderBy, order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc);

        for(var indice = 0; indice< orderedList.Count; indice++)
        {
            var outputItem = output.Data[indice];
            var exampleItem = orderedList[indice];
            outputItem.Should().NotBeNull();
            exampleItem.Should().NotBeNull();
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(exampleItem.CreatedAt.TrimMilliSeconds());
        }
    }

    public void Dispose()
    {
        this.fixture.ClearPersistence();
    }
}