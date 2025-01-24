// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CastMembers.GetCastMember;

using System.Net;
using Common;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(CastMemberApiBaseFixture))]
public class GetCastMemberApiTest(CastMemberApiBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Get))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task Get()
    {
        var examples = fixture.GetExampleCastMembersList(5);
        var example = examples[2];
        await fixture.CastMemberPersistence.InsertList(examples);

        var (response, output) =
            await fixture.ApiClient.Get<ApiResponse<CastMemberModelOutput>>($"castMembers/{example.Id}");

        response.EnsureSuccessStatusCode();
        response.Should().NotBeNull();
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(example.Id);
        output.Data.Name.Should().Be(example.Name);
        output.Data.Type.Should().Be(example.Type);

    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task NotFound()
    {
        var example = Guid.NewGuid();

        var (response, output) =
            await fixture.ApiClient.Get<ProblemDetails>($"castmembers/{example}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{example}' not found.");
        output.Type.Should().Be("NotFound");

    }


}