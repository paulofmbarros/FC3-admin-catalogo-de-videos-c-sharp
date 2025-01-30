// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.ValueObjects;

using Common;
using Fc.CodeFlix.Catalog.Domain.ValueObject;
using FluentAssertions;

public class ImageTest : BaseFixture
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Image - Value Objects")]
    public void Instantiate()
    {
        var path = Faker.Image.PicsumUrl();
        var image = new Image(path);
        image.Path.Should().Be(path);
    }

    [Fact(DisplayName = nameof(EqualsByPath))]
    [Trait("Domain", "Image - Value Objects")]
    public void EqualsByPath()
    {
        var path = this.Faker.Image.PicsumUrl();
        var image = new Image(path);
        var sameImage = new Image(path);

        var isItEquals = image == sameImage;
        isItEquals.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(DifferentByPath))]
    [Trait("Domain", "Image - Value Objects")]
    public void DifferentByPath()
    {
        var path = this.Faker.Image.PicsumUrl();
        var image = new Image(path);
        var differentPath = this.Faker.Image.PicsumUrl();
        var differentImage = new Image(differentPath);

        var isItDifferent = image != differentImage;
        isItDifferent.Should().BeTrue();
    }
}