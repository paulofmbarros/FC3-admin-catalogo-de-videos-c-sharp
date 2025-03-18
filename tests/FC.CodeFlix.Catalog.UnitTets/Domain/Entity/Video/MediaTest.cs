// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Video;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using FluentAssertions;

[Collection(nameof(VideoTestFixture))]
public class MediaTest
{
    private readonly VideoTestFixture fixture;

    public MediaTest(VideoTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Media - Entities")]
    public void Instantiate()
    {
        var expectedFilePath = this.fixture.GetValidMediaPath();

        var media = new Media(expectedFilePath);

        media.FilePath.Should().Be(expectedFilePath);
        media.Id.Should().NotBeEmpty(default);
        media.Status.Should().Be(MediaStatus.Pending);

    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsSentToEncode()
    {
        var media = this.fixture.GetValidMedia();

        media.UpdateAsSentToEncode();
        media.Status.Should().Be(MediaStatus.Processing);

    }

    [Fact(DisplayName = nameof(UpdateAsEncoded))]
    [Trait("Domain", "Media - Entities")]
    public void UpdateAsEncoded()
    {
        var media = this.fixture.GetValidMedia();
        var encodedExamplePath = this.fixture.GetValidMediaPath();
        media.UpdateAsSentToEncode();

        media.UpdateAsEncoded(encodedExamplePath);
        media.Status.Should().Be(MediaStatus.Completed);
        media.EncodedPath.Should().Be(encodedExamplePath);


    }
}