// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CastMembers.DeleteCastMember;

using System.Net;
using Common;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(CastMemberApiBaseFixture))]
public class DeleteCastMemberApiTest
{
    private readonly CastMemberApiBaseFixture fixture;

    public DeleteCastMemberApiTest(CastMemberApiBaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task Delete()
    {
        var examples = fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        await fixture.CastMemberPersistence.InsertList(examples);

        var (response, output) =
            await fixture.ApiClient.Delete<object>($"castMembers/{example.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var castMember = await this.fixture.CastMemberPersistence.GetById(example.Id);
        castMember.Should().BeNull();
    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task NotFound()
    {
        var exampleGuid = Guid.NewGuid();

        var (response, output) =
            await fixture.ApiClient.Get<ProblemDetails>($"castMembers/{exampleGuid}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{exampleGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }

}