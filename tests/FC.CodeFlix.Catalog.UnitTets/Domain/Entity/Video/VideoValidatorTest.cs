// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Domain.Entity.Video;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Validation;
using Fc.CodeFlix.Catalog.Domain.Validator;
using FluentAssertions;

[Collection(nameof(VideoTestFixture))]
public class VideoValidatorTest
{
    private readonly VideoTestFixture fixture;

    public VideoValidatorTest(VideoTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(ReturnsValidWhenVideoIsValid))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsValidWhenVideoIsValid()
    {
        var validVideo = this.fixture.GetValidVideo();
        var notificationValidationHandler = new NotificationValidationHandler();

        var videoValidator = new VideoValidator(validVideo, notificationValidationHandler);
        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeFalse();
        notificationValidationHandler.Errors.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(ReturnsErrorWhenTitleIsTooLong))]
    [Trait("Domain", "Video Validator - Validators")]
    public void ReturnsErrorWhenTitleIsTooLong()
    {
        var invalidVideo = new Video(
            this.fixture.GetTooLongTitle(),
            this.fixture.GetValidDescription(),
            this.fixture.GetRandomBoolean(),
            this.fixture.GetRandomBoolean(),
            this.fixture.GetValidYearLaunched(),
            this.fixture.GetValidDuration()
            );
        var notificationValidationHandler = new NotificationValidationHandler();

        var videoValidator = new VideoValidator(invalidVideo, notificationValidationHandler);
        videoValidator.Validate();

        notificationValidationHandler.HasErrors().Should().BeTrue();
        notificationValidationHandler.Errors.Should().HaveCount(1);
        notificationValidationHandler.Errors.First().Message.Should().Be("Title should be less or equal 255 characters long.");

    }
}