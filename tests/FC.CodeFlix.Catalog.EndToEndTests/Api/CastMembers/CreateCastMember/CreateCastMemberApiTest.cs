// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CastMembers.CreateCastMember;

using System.Net;
using Common;
using Extensions.DateTime;
using Fc.CodeFlix.Catalog.Api.ApiModels.Response;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[Collection(nameof(CastMemberApiBaseFixture))]
public class CreateCastMemberApiTest(CastMemberApiBaseFixture fixture)
{
    [Fact(DisplayName = nameof(Create))]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task Create()
    {
        var castMember = fixture.GetExampleCastMember();

        var (response, output) =
            await fixture.ApiClient.Post<ApiResponse<CastMemberModelOutput>>("castMembers", castMember);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(castMember.Name);
        output.Data.Type.Should().Be(castMember.Type);
        output.Data.CreatedAt.Should().NotBe(default);

        var castMemberInPersistence = await fixture.CastMemberPersistence.GetById(output.Data.Id);
        castMemberInPersistence.Should().NotBeNull();
        castMemberInPersistence.Id.Should().Be(output.Data.Id);
        castMemberInPersistence.Name.Should().Be(output.Data.Name);
        castMemberInPersistence.Type.Should().Be(output.Data.Type);
        castMemberInPersistence.CreatedAt.TrimMilliSeconds().Should().Be(output.Data.CreatedAt.TrimMilliSeconds());


    }

    [Theory(DisplayName = nameof(CreateFailsValidation))]
    [InlineData("")]
    [InlineData(" ")]
    [Trait("EndToEnd/Api", "Category/CastMember - Endpoints")]
    public async Task CreateFailsValidation(string? name)
    {
        var castMemberInput = new CreateCastMemberInput(name, CastMemberType.Actor);

        var (response, output) =
            await fixture.ApiClient.Post<ProblemDetails>("castMembers", castMemberInput);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Title.Should().Be("One or more validation errors occurred.");
        output.Detail.Should().Be("Name should not be empty or null.");
        output.Type.Should().Be("UnprocessableEntity");

    }
}