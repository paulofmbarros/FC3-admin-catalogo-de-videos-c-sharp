// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Video;

using Fc.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;

[Collection(nameof(VideoTestFixture))]
public class VideoTest
{
    private readonly VideoTestFixture fixture;

    public VideoTest(VideoTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Video - Aggregate")]
    public void Instantiate()
    {
        var expectedTitle = this.fixture.GetValidTitle();
        var expectedDescription = this.fixture.GetValidDescription();
        var expectedOpened = this.fixture.GetRandomBoolean();
        var expectedPublished = this.fixture.GetRandomBoolean();
        var expectedYearLaunched = this.fixture.GetValidYearLaunched();
        var expectedDuration = this.fixture.GetValidDuration();


        var video = new Video(
            expectedTitle,
            expectedDescription,
            expectedOpened,
            expectedPublished,
            expectedYearLaunched,
            expectedDuration
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.Opened.Should().BeTrue();
        video.Published.Should().BeTrue();
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);

    }
}