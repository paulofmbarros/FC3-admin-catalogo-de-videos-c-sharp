// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Domain.Entity;
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

    [Fact(DisplayName = nameof(InsertWithRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task InsertWithRelations()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetExampleVideo();
        var castMembers = this.fixture.GetRandomCastMemberList();
        castMembers.ToList().ForEach(castMember =>exampleVideo.AddCastMember(castMember.Id));
        await dbContext.CastMembers.AddRangeAsync(castMembers);
        var categories = fixture.GetRandomCategoryList();
        categories.ToList().ForEach(category =>exampleVideo.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categories);
        var genres = fixture.GetRandomGenresList();
        genres.ToList().ForEach(genre => exampleVideo.AddGenre(genre.Id));
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync();

        var videoRepository = new VideoRepository(dbContext);
        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = this.fixture.CreateDbContext(true);

        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        var dbVideosCategories = assertsDbContext.VideosCategories
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosCategories.Should().HaveCount(categories.Count);
        dbVideosCategories.Select(relation => relation.CategoryId).ToList()
            .Should().BeEquivalentTo(
                categories.Select(category => category.Id));
        var dbVideosGenres = assertsDbContext.VideosGenres
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosGenres.Should().HaveCount(genres.Count);
        dbVideosGenres.Select(relation => relation.GenreId).ToList()
            .Should().BeEquivalentTo(
                genres.Select(genre => genre.Id));
        var dbVideosCastMembers = assertsDbContext.VideosCastMembers
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosCastMembers.Should().HaveCount(castMembers.Count);
        dbVideosCastMembers.Select(relation => relation.CastMemberId).ToList()
            .Should().BeEquivalentTo(
                castMembers.Select(castMember => castMember.Id));

    }

    [Fact(DisplayName = nameof(InsertWithMediasAndImages))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task InsertWithMediasAndImages()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();


        var videoRepository = new VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = this.fixture.CreateDbContext(true);

        var dbVideo = await assertsDbContext.Videos
            .Include(x=> x.Media)
            .Include(x=> x.Trailer)
            .FirstOrDefaultAsync(x=> x.Id == exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo.Id.Should().Be(exampleVideo.Id);
        dbVideo.Thumb.Should().NotBeNull();
        dbVideo.Thumb?.Path.Should().Be(exampleVideo.Thumb.Path);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf?.Path.Should().Be(exampleVideo.ThumbHalf.Path);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner?.Path.Should().Be(exampleVideo.Banner?.Path);
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Media?.FilePath.Should().Be(exampleVideo.Media.FilePath);
        dbVideo.Media?.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        dbVideo.Media?.Status.Should().Be(exampleVideo.Media.Status);
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Trailer?.FilePath.Should().Be(exampleVideo.Trailer.FilePath);
        dbVideo.Trailer?.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
        dbVideo.Trailer?.Status.Should().Be(exampleVideo.Trailer.Status);

    }
}