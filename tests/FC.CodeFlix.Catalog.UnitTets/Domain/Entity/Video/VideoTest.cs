// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Video;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Validation;
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
        var expectedRating = Rating.Er;


        var video = new Video(
            expectedTitle,
            expectedDescription,
            expectedOpened,
            expectedPublished,
            expectedYearLaunched,
            expectedDuration,
            expectedRating
        );

        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);

    }

    [Fact(DisplayName = nameof(ValidateWhenValidState))]
    [Trait("Domain", "Video - Aggregate")]
    public void ValidateWhenValidState()
    {
        var video = this.fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        video.Validate(notificationValidationHandler);

        notificationValidationHandler.HasErrors().Should().BeFalse();

    }

    [Fact(DisplayName = nameof(ValidateWithErrorsWhenInvalidState))]
    [Trait("Domain", "Video - Aggregate")]
    public void ValidateWithErrorsWhenInvalidState()
    {
        var video = this.fixture.GetInvalidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        video.Validate(notificationValidationHandler);

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(2);
        notificationValidationHandler.Errors.Should().Contain(x => x.Message == "Title should be less or equal 255 characters long.");
        notificationValidationHandler.Errors.Should().Contain(x => x.Message == "Description should be less or equal 4000 characters long.");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Video - Aggregate")]
    public void Update()
    {
        var video = this.fixture.GetInvalidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        var expectedTitle = this.fixture.GetValidTitle();
        var expectedDescription = this.fixture.GetValidDescription();
        var expectedOpened = this.fixture.GetRandomBoolean();
        var expectedPublished = this.fixture.GetRandomBoolean();
        var expectedYearLaunched = this.fixture.GetValidYearLaunched();
        var expectedDuration = this.fixture.GetValidDuration();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedOpened,
            expectedPublished,
            expectedYearLaunched,
            expectedDuration
        );
        video.Validate(notificationValidationHandler);

        notificationValidationHandler.HasErrors().Should().BeFalse();
        video.Title.Should().Be(expectedTitle);
        video.Description.Should().Be(expectedDescription);
        video.Opened.Should().Be(expectedOpened);
        video.Published.Should().Be(expectedPublished);
        video.YearLaunched.Should().Be(expectedYearLaunched);
        video.Duration.Should().Be(expectedDuration);
    }

    [Fact(DisplayName = nameof(ValidateGenerateErrorsAfterUpdateToInvalidState))]
    [Trait("Domain", "Video - Aggregate")]
    public void ValidateGenerateErrorsAfterUpdateToInvalidState()
    {
        var video = this.fixture.GetInvalidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();
        var expectedTitle = this.fixture.GetTooLongTitle();
        var expectedDescription = this.fixture.GetTooLongDescription();
        var expectedOpened = this.fixture.GetRandomBoolean();
        var expectedPublished = this.fixture.GetRandomBoolean();
        var expectedYearLaunched = this.fixture.GetValidYearLaunched();
        var expectedDuration = this.fixture.GetValidDuration();

        video.Update(
            expectedTitle,
            expectedDescription,
            expectedOpened,
            expectedPublished,
            expectedYearLaunched,
            expectedDuration
        );
        video.Validate(notificationValidationHandler);

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(2);
        notificationValidationHandler.Errors.Should().Contain(x => x.Message == "Title should be less or equal 255 characters long.");
        notificationValidationHandler.Errors.Should().Contain(x => x.Message == "Description should be less or equal 4000 characters long.");

    }



}