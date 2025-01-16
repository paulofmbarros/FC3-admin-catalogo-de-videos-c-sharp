// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.CastMember;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Validation;
using FluentAssertions;

[Collection(nameof(CastMemberFixture))]
public class CastMemberTest
{
    private readonly CastMemberFixture fixture;

    public CastMemberTest(CastMemberFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Instantiate()
    {
        var dateTimeBefore = DateTime.Now.AddSeconds(-1);
        var name = this.fixture.GetValidName();
        var type = this.fixture.GetRandomCastMemberType();

        var castMember = new CastMember(name, type);

        var dateTimeAfter = DateTime.Now.AddSeconds(1);

        castMember.Id.Should().NotBeEmpty();
        castMember.Id.Should().NotBe(default(Guid));
        castMember.Name.Should().Be(name);
        (castMember.CreatedAt >= dateTimeBefore).Should().BeTrue();
        (castMember.CreatedAt <= dateTimeAfter).Should().BeTrue();
        castMember.Type.Should().Be(type);
    }

    [Theory(DisplayName = nameof(ThrowErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowErrorWhenNameIsInvalid(string? name)
    {
        var type = this.fixture.GetRandomCastMemberType();

        var action = () => new CastMember(name!, type);

        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");

    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "CastMember - Aggregates")]
    public void Update()
    {
        var newName = this.fixture.GetValidName();
        var newType = this.fixture.GetRandomCastMemberType();
        var castMember = this.fixture.GetExampleCastMember();

        castMember.Update(newName, newType);


        castMember.Id.Should().NotBeEmpty();
        castMember.Id.Should().NotBe(default(Guid));
        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
    }

    [Theory(DisplayName = nameof(UpdateThrowErrorWhenNameIsInvalid))]
    [Trait("Domain", "CastMember - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateThrowErrorWhenNameIsInvalid(string? newName)
    {
        var castMember = this.fixture.GetExampleCastMember();
        var newType = this.fixture.GetRandomCastMemberType();

        var action = () => castMember.Update(newName!, newType);

        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null.");

    }




}