// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CastMembers.UpdateCastMember;

using System.Net;
using Common;
using Extensions.DateTime;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using Fc.CodeFlix.Catalog.Domain.Enum;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(CastMemberApiBaseFixture))]
public class UpdateCastMemberApiTest
{
    private readonly CastMemberApiBaseFixture fixture;

    public UpdateCastMemberApiTest(CastMemberApiBaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task Update()
    {
        var examplesCastMembers = fixture.GetExampleCastMembersList(5);
        var exampleCastMember = examplesCastMembers[2];
        await this.fixture.CastMemberPersistence.InsertList(examplesCastMembers);

        var castMemberUpdateInput = new UpdateCastMemberInput(exampleCastMember.Id, fixture.GetValidName(), this.fixture.GetRandomCastMemberType());

        var (response, output) =
            await fixture.ApiClient.Put<ApiResponse<CastMemberModelOutput>>($"castMembers/{exampleCastMember.Id}", castMemberUpdateInput);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(castMemberUpdateInput.Id);
        output.Data.Name.Should().Be(castMemberUpdateInput.Name);
        output.Data.Type.Should().Be(castMemberUpdateInput.Type);
        output.Data.CreatedAt.TrimMilliSeconds().Should().Be(exampleCastMember.CreatedAt.TrimMilliSeconds());

        var castMemberInPersistence = await fixture.CastMemberPersistence.GetById(output.Data.Id);
        castMemberInPersistence.Should().NotBeNull();
        castMemberInPersistence.Id.Should().Be(output.Data.Id);
        castMemberInPersistence.Name.Should().Be(output.Data.Name);
        castMemberInPersistence.Type.Should().Be(output.Data.Type);

    }

    [Fact(DisplayName = nameof(NotFound))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task NotFound()
    {
        var randomGuid = Guid.NewGuid();
        var input = new UpdateCastMemberInput(randomGuid, fixture.GetValidName(), this.fixture.GetRandomCastMemberType());
        var (response, output) =
            await fixture.ApiClient.Put<ProblemDetails>($"castMembers/{randomGuid}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"CastMember '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");

    }

    [Fact(DisplayName = nameof(UpdateThrowWhenFailValidation))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task UpdateThrowWhenFailValidation()
    {
        var examplesCastMembers = fixture.GetExampleCastMembersList(5);
        var exampleCastMember = examplesCastMembers[2];
        await this.fixture.CastMemberPersistence.InsertList(examplesCastMembers);

        var castMemberUpdateInput = new UpdateCastMemberInput(exampleCastMember.Id, "", this.fixture.GetRandomCastMemberType());
        var (response, output) =
            await fixture.ApiClient.Put<ProblemDetails>($"castMembers/{exampleCastMember.Id}", castMemberUpdateInput);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Title.Should().Be("One or more validation errors occurred.");
        output.Detail.Should().Be("Name should not be empty or null.");
        output.Type.Should().Be("UnprocessableEntity");


    }


}