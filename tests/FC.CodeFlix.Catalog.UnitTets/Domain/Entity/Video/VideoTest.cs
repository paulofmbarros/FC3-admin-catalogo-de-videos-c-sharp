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
        video.Thumb.Should().BeNull();
        video.ThumbHalf.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();

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

    [Fact(DisplayName = nameof(UpdateThumb))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateThumb()
    {
        var video = this.fixture.GetValidVideo();
        var validImagePath = this.fixture.GetValidImagePath();

        video.UpdateThumb(validImagePath);
        video.Thumb.Path.Should().NotBeNull();
        video.Thumb.Path.Should().Be(validImagePath);

    }

    [Fact(DisplayName = nameof(UpdateThumbHalf))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateThumbHalf()
    {
        var video = this.fixture.GetValidVideo();
        var validImagePath = this.fixture.GetValidImagePath();

        video.UpdateThumbHalf(validImagePath);
        video.ThumbHalf.Path.Should().NotBeNull();
        video.ThumbHalf.Path.Should().Be(validImagePath);

    }

    [Fact(DisplayName = nameof(UpdateBanner))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateBanner()
    {
        var video = this.fixture.GetValidVideo();
        var validImagePath = this.fixture.GetValidImagePath();

        video.UpdateBanner(validImagePath);
        video.Banner.Path.Should().NotBeNull();
        video.Banner.Path.Should().Be(validImagePath);

    }

    [Fact(DisplayName = nameof(UpdateMedia))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateMedia()
    {
        var video = this.fixture.GetValidVideo();
        var validMediaPath = this.fixture.GetValidMediaPath();

        video.UpdateMedia(validMediaPath);
        video.Media.Should().NotBeNull();
        video.Media.FilePath.Should().Be(validMediaPath);

    }

    [Fact(DisplayName = nameof(UpdateTrailer))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateTrailer()
    {
        var video = this.fixture.GetValidVideo();
        var validMediaPath = this.fixture.GetValidMediaPath();

        video.UpdateTrailer(validMediaPath);
        video.Trailer.Should().NotBeNull();
        video.Trailer.FilePath.Should().Be(validMediaPath);

    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncode))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateAsSentToEncode()
    {
        var video = this.fixture.GetValidVideo();
        var validPath = this.fixture.GetValidMediaPath();
        video.UpdateMedia(validPath);

        video.UpdateAsSentToEncode();
        video.Media.Status.Should().Be(MediaStatus.Processing);

    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Video - Aggregate")]
    public void AddCategory()
    {
        var video = this.fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();
        video.AddCategory(categoryIdExample);

        video.Categories.Should().HaveCount(1);
        video.Categories[0].Should().Be(categoryIdExample);

    }

    [Fact(DisplayName = nameof(UpdateAsSentToEncodeThrowsWhenThereIsNoMedia))]
    [Trait("Domain", "Video - Aggregate")]
    public void UpdateAsSentToEncodeThrowsWhenThereIsNoMedia()
    {
        var video = this.fixture.GetValidVideo();

        var action = () => video.UpdateAsSentToEncode();

        action.Should().Throw<EntityValidationException>()
            .WithMessage("There is no media");

    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveCategory()
    {
        var video = this.fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();
        var categoryIdExample2 = Guid.NewGuid();
        video.AddCategory(categoryIdExample);
        video.AddCategory(categoryIdExample2);

        video.RemoveCategory(categoryIdExample);

        video.Categories.Should().HaveCount(1);
        video.Categories[0].Should().Be(categoryIdExample2);

    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveAllCategories()
    {
        var video = this.fixture.GetValidVideo();
        var categoryIdExample = Guid.NewGuid();
        var categoryIdExample2 = Guid.NewGuid();
        video.AddCategory(categoryIdExample);
        video.AddCategory(categoryIdExample2);

        video.RemoveAllCategories();

        video.Categories.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(AddGenre))]
    [Trait("Domain", "Video - Aggregate")]
    public void AddGenre()
    {
        var video = this.fixture.GetValidVideo();
        var genreIdExample = Guid.NewGuid();
        video.AddGenre(genreIdExample);

        video.Genres.Should().HaveCount(1);
        video.Genres[0].Should().Be(genreIdExample);

    }

    [Fact(DisplayName = nameof(RemoveGenre))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveGenre()
    {
        var video = this.fixture.GetValidVideo();
        var genreIdExample = Guid.NewGuid();
        var genreIdExample2 = Guid.NewGuid();
        video.AddGenre(genreIdExample);
        video.AddGenre(genreIdExample2);

        video.RemoveGenre(genreIdExample2);

        video.Genres.Should().HaveCount(1);
        video.Genres[0].Should().Be(genreIdExample);

    }

    [Fact(DisplayName = nameof(RemoveAllGenres))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveAllGenres()
    {
        var video = this.fixture.GetValidVideo();
        var genreIdExample = Guid.NewGuid();
        var genreIdExample2 = Guid.NewGuid();
        video.AddGenre(genreIdExample);
        video.AddGenre(genreIdExample2);

        video.RemoveAllGenres();

        video.Genres.Should().HaveCount(0);

    }

    [Fact(DisplayName = nameof(AddCastMember))]
    [Trait("Domain", "Video - Aggregate")]
    public void AddCastMember()
    {
        var video = this.fixture.GetValidVideo();
        var castMemberIdExample = Guid.NewGuid();
        video.AddCastMember(castMemberIdExample);

        video.CastMembers.Should().HaveCount(1);
        video.CastMembers[0].Should().Be(castMemberIdExample);

    }

    [Fact(DisplayName = nameof(RemoveCastMember))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveCastMember()
    {
        var video = this.fixture.GetValidVideo();
        var castMemberIdExample = Guid.NewGuid();
        var castMemberIdExample2 = Guid.NewGuid();
        video.AddCastMember(castMemberIdExample);
        video.AddCastMember(castMemberIdExample2);

        video.RemoveCastMember(castMemberIdExample2);

        video.CastMembers.Should().HaveCount(1);
        video.CastMembers[0].Should().Be(castMemberIdExample);

    }

    [Fact(DisplayName = nameof(RemoveAllCastMembers))]
    [Trait("Domain", "Video - Aggregate")]
    public void RemoveAllCastMembers()
    {
        var video = this.fixture.GetValidVideo();
        var castMemberIdExample = Guid.NewGuid();
        var castMemberIdExample2 = Guid.NewGuid();
        video.AddCastMember(castMemberIdExample);
        video.AddCastMember(castMemberIdExample2);

        video.RemoveAllCastMembers();

        video.CastMembers.Should().HaveCount(0);

    }

}