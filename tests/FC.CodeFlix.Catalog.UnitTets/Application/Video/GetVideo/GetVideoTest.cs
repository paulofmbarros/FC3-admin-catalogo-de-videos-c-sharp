// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.GetVideo;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.GetVideo;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Fc.CodeFlix.Catalog.Domain.SeedWork;
using FluentAssertions;
using Moq;

[Collection(nameof(GetVideoTestFixture))]
public class GetVideoTest
{
    private readonly GetVideoTestFixture fixture;

    public GetVideoTest(GetVideoTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(Get))]
    [Trait("Application ", "GetVideo - Use Case")]
    public async Task Get()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var repositoryMock = new Mock<IVideoRepository>();

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(id => id ==exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var useCase = new GetVideo(repositoryMock.Object);
        var input = new GetVideoInput(exampleVideo.Id);

      var output = await useCase.Handle(input, new CancellationToken());

      output.Id.Should().NotBeEmpty();
      output.CreatedAt.Should().NotBe(default);
      output.Title.Should().Be(exampleVideo.Title);
      output.Published.Should().Be(exampleVideo.Published);
      output.Description.Should().Be(exampleVideo.Description);
      output.Duration.Should().Be(exampleVideo.Duration);
      output.Rating.Should().Be(exampleVideo.Rating);
      output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
      output.Opened.Should().Be(exampleVideo.Opened);

    }

    [Fact(DisplayName = nameof(ThrowsExceptionWhenNotFound))]
    [Trait("Application ", "GetVideo - Use Case")]
    public async Task ThrowsExceptionWhenNotFound()
    {
        var repositoryMock = new Mock<IVideoRepository>();

        repositoryMock.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        var useCase = new GetVideo(repositoryMock.Object);
        var input = new GetVideoInput(Guid.NewGuid());

        var action = async () => await useCase.Handle(input, new CancellationToken());

        await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video not found");
        repositoryMock.VerifyAll();

    }

    [Fact(DisplayName = nameof(GetWithAllProperties))]
    [Trait("Application ", "GetVideo - Use Case")]
    public async Task GetWithAllProperties()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var repositoryMock = new Mock<IVideoRepository>();

        repositoryMock.Setup(x => x.Get(It.Is<Guid>(id => id ==exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var useCase = new GetVideo(repositoryMock.Object);
        var input = new GetVideoInput(exampleVideo.Id);

        var output = await useCase.Handle(input, new CancellationToken());

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(exampleVideo.Title);
        output.Published.Should().Be(exampleVideo.Published);
        output.Description.Should().Be(exampleVideo.Description);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.Rating.Should().Be(exampleVideo.Rating);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Thumb.Should().Be(exampleVideo.Thumb.Path);
        output.ThumbHalf.Should().Be(exampleVideo.ThumbHalf.Path);
        output.Banner.Should().Be(exampleVideo.Banner.Path);
        output.Media.Should().Be(exampleVideo.Media.FilePath);
        output.Trailer.Should().Be(exampleVideo.Trailer.FilePath);
        output.CategoriesIds.Should().NotBeEmpty();
        output.CategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
        output.CastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
        output.GenresIds.Should().BeEquivalentTo(exampleVideo.Genres);

        repositoryMock.VerifyAll();

    }
}