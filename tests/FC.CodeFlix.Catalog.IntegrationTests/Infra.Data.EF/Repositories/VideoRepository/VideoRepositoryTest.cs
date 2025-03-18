// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

[Collection(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTest
{
    private readonly VideoRepositoryTestFixture fixture;

    public VideoRepositoryTest(VideoRepositoryTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Insert()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetExampleVideo();


        var videoRepository = new VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = this.fixture.CreateDbContext(true);

        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Id.Should().Be(exampleVideo.Id);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        dbVideo.Rating.Should().Be(exampleVideo.Rating);
        dbVideo.Thumb?.Path.Should().BeNull();
        dbVideo.ThumbHalf?.Path.Should().BeNull();
        dbVideo.Banner?.Path.Should().BeNull();
        dbVideo.Media?.FilePath.Should().BeNull();
        dbVideo.Trailer?.FilePath.Should().BeNull();
        dbVideo.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();

    }
}