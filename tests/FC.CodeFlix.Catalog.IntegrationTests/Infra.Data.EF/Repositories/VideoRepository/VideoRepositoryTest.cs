// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

using Catalog.Infra.Data.EF;
using Catalog.Infra.Data.EF.Models;
using Catalog.Infra.Data.EF.Repositories;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Enum;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
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
        castMembers.ToList().ForEach(castMember => exampleVideo.AddCastMember(castMember.Id));
        await dbContext.CastMembers.AddRangeAsync(castMembers);
        var categories = fixture.GetRandomCategoryList();
        categories.ToList().ForEach(category => exampleVideo.AddCategory(category.Id));
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
            .Include(x => x.Media)
            .Include(x => x.Trailer)
            .FirstOrDefaultAsync(x => x.Id == exampleVideo.Id);
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

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Update()
    {
        var dbContextArrange = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetExampleVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();

        var dbContextAct = fixture.CreateDbContext(true);
        var newValuesVideo = this.fixture.GetExampleVideo();

        var videoRepository = new VideoRepository(dbContextAct);
        
        exampleVideo.Update(newValuesVideo.Title,
            newValuesVideo.Description,
            newValuesVideo.Opened,
            newValuesVideo.Published,
            newValuesVideo.YearLaunched,
            newValuesVideo.Duration,
            newValuesVideo.Rating);
        
        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

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

     [Fact(DisplayName = nameof(UpdateEntitiesAndValueObjects))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateEntitiesAndValueObjects()
    {
        var dbContextArrange = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetExampleVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();

        var dbContextAct = fixture.CreateDbContext(true);
        var updatedThumb = fixture.GetValidImagePath();
        var updatedThumbHalf = fixture.GetValidImagePath();
        var updatedBanner = fixture.GetValidImagePath();
        var updatedMedia = fixture.GetValidMediaPath();
        var updatedTrailer = fixture.GetValidMediaPath();
        var updatedMediaEncoded = fixture.GetValidMediaPath();
        var videoRepository = new VideoRepository(dbContextAct);
        var savedVideo = await dbContextAct.Videos.FindAsync(exampleVideo.Id);


        savedVideo.UpdateBanner(updatedBanner);
        savedVideo.UpdateThumb(updatedThumb);
        savedVideo.UpdateThumbHalf(updatedThumbHalf);
        savedVideo.UpdateMedia(updatedMedia);
        savedVideo.UpdateAsEncoded(updatedMediaEncoded);
        savedVideo.UpdateTrailer(updatedTrailer);

        await videoRepository.Update(savedVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = this.fixture.CreateDbContext(true);

        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo.Thumb?.Should().NotBeNull();
        dbVideo.Thumb?.Path.Should().Be(updatedThumb);
        dbVideo.ThumbHalf?.Should().NotBeNull();
        dbVideo.ThumbHalf?.Path.Should().Be(updatedThumbHalf);
        dbVideo.Banner?.Should().NotBeNull();
        dbVideo.Banner?.Path.Should().Be(updatedBanner);
        dbVideo.Media?.FilePath.Should().Be(updatedMedia);
        dbVideo.Media?.EncodedPath.Should().Be(updatedMediaEncoded);
        dbVideo.Media?.Status.Should().Be(MediaStatus.Completed);
        dbVideo.Trailer?.FilePath.Should().Be(updatedTrailer);
    }


    [Fact(DisplayName = nameof(UpdateWithRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task UpdateWithRelations()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetExampleVideo();
        await dbContext.Videos.AddAsync(exampleVideo);

        var castMembers = this.fixture.GetRandomCastMemberList();
        var categories = fixture.GetRandomCategoryList();
        var genres = fixture.GetRandomGenresList();

        await dbContext.CastMembers.AddRangeAsync(castMembers);
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = fixture.CreateDbContext(true);
        var savedVideo= await actDbContext.Videos.FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);

        var videoRepository = new VideoRepository(actDbContext);
        castMembers.ToList().ForEach(castMember => savedVideo.AddCastMember(castMember.Id));
        categories.ToList().ForEach(category => savedVideo.AddCategory(category.Id));
        genres.ToList().ForEach(genre => savedVideo.AddGenre(genre.Id));
        await videoRepository.Update(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

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

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Delete()
    {
        var id = Guid.Empty;
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetExampleVideo();
        id = exampleVideo.Id;
        await dbContext.Videos.AddAsync(exampleVideo);

        var castMembers = this.fixture.GetRandomCastMemberList();
        var categories = fixture.GetRandomCategoryList();
        var genres = fixture.GetRandomGenresList();

        await dbContext.CastMembers.AddRangeAsync(castMembers);
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = fixture.CreateDbContext(true);
        var savedVideo= await actDbContext.Videos.FirstOrDefaultAsync(video => video.Id == id);

        var videoRepository = new VideoRepository(actDbContext);
        await videoRepository.Delete(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = this.fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(id);
        dbVideo.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteWithAllPropertiesAndRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task DeleteWithAllPropertiesAndRelations()
    {
        var id = Guid.Empty;
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        id = exampleVideo.Id;
        await dbContext.Videos.AddAsync(exampleVideo);

        var castMembers = this.fixture.GetRandomCastMemberList();
        var categories = fixture.GetRandomCategoryList();
        var genres = fixture.GetRandomGenresList();

        await dbContext.CastMembers.AddRangeAsync(castMembers);
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = fixture.CreateDbContext(true);
        var savedVideo= await actDbContext.Videos.FirstOrDefaultAsync(video => video.Id == id);

        var videoRepository = new VideoRepository(actDbContext);
        castMembers.ToList().ForEach(castMember => savedVideo.AddCastMember(castMember.Id));
        categories.ToList().ForEach(category => savedVideo.AddCategory(category.Id));
        genres.ToList().ForEach(genre => savedVideo.AddGenre(genre.Id));
        await videoRepository.Delete(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = this.fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(id);
        dbVideo.Should().BeNull();
        var dbVideosCategories = assertsDbContext.VideosCategories
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosCategories.Should().HaveCount(0);
        var dbVideosGenres = assertsDbContext.VideosGenres
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosGenres.Should().HaveCount(0);
        var dbVideosCastMembers = assertsDbContext.VideosCastMembers
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosCastMembers.Should().HaveCount(0);
        assertsDbContext.Set<Media>().Count().Should().Be(0);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Get()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetExampleVideo();
        await dbContext.Videos.AddAsync(exampleVideo);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = fixture.CreateDbContext(true);

        var videoRepository = new VideoRepository(actDbContext);
        var video = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

        video.Should().NotBeNull();
        video.Title.Should().Be(exampleVideo.Title);
        video.Id.Should().Be(exampleVideo.Id);
        video.Description.Should().Be(exampleVideo.Description);
        video.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        video.Opened.Should().Be(exampleVideo.Opened);
        video.Published.Should().Be(exampleVideo.Published);
        video.Duration.Should().Be(exampleVideo.Duration);
        video.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        video.Rating.Should().Be(exampleVideo.Rating);
        video.Thumb?.Path.Should().BeNull();
        video.ThumbHalf?.Path.Should().BeNull();
        video.Banner?.Path.Should().BeNull();
        video.Media?.FilePath.Should().BeNull();
        video.Trailer?.FilePath.Should().BeNull();
        video.Genres.Should().BeEmpty();
        video.Categories.Should().BeEmpty();
        video.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(GetThrowIfNotFound))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task GetThrowIfNotFound()
    {
        var id = Guid.NewGuid();
        var actDbContext = fixture.CreateDbContext(true);

        var videoRepository = new VideoRepository(actDbContext);
        var action = async () => await videoRepository.Get(id, CancellationToken.None);

        action.Should().ThrowAsync<NotFoundException>().WithMessage($"Video with '{id}' not found");
    }

    [Fact(DisplayName = nameof(GetWithAllProperties))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task GetWithAllProperties()
    {
        var dbContext = fixture.CreateDbContext();
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        await dbContext.Videos.AddAsync(exampleVideo);

        var castMembers = this.fixture.GetRandomCastMemberList();
        var categories = fixture.GetRandomCategoryList();
        var genres = fixture.GetRandomGenresList();

        await dbContext.CastMembers.AddRangeAsync(castMembers);
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.Genres.AddRangeAsync(genres);
        castMembers.ToList().ForEach(castMember =>
        {
            exampleVideo.AddCastMember(castMember.Id);
            dbContext.VideosCastMembers.Add(new(castMember.Id,exampleVideo.Id));
        });
        categories.ToList().ForEach(category =>
        {
            exampleVideo.AddCategory(category.Id);
            dbContext.VideosCategories.Add(new(category.Id,exampleVideo.Id));
        });
        genres.ToList().ForEach(genre =>
        {
            exampleVideo.AddGenre(genre.Id);
            dbContext.VideosGenres.Add(new(genre.Id,exampleVideo.Id));
        });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = fixture.CreateDbContext(true);
        var videoRepository = new VideoRepository(actDbContext);
        var video = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

        video.Should().NotBeNull();
        video.Title.Should().Be(exampleVideo.Title);
        video.Id.Should().Be(exampleVideo.Id);
        video.Description.Should().Be(exampleVideo.Description);
        video.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        video.Opened.Should().Be(exampleVideo.Opened);
        video.Published.Should().Be(exampleVideo.Published);
        video.Duration.Should().Be(exampleVideo.Duration);
        video.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        video.Rating.Should().Be(exampleVideo.Rating);
        video.Thumb?.Path.Should().NotBeNull();
        video.Thumb?.Path.Should().Be(exampleVideo.Thumb?.Path);
        video.ThumbHalf?.Path.Should().NotBeNull();
        video.ThumbHalf?.Path.Should().Be(exampleVideo.ThumbHalf.Path);
        video.Banner?.Path.Should().NotBeNull();
        video.Banner?.Path.Should().NotBeNull(exampleVideo.Banner?.Path);
        video.Media?.FilePath.Should().NotBeNull();
        video.Media?.FilePath.Should().Be(exampleVideo.Media.FilePath);
        video.Media?.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        video.Media?.Status.Should().Be(exampleVideo.Media.Status);
        video.Trailer?.FilePath.Should().NotBeNull();
        video.Trailer?.FilePath.Should().Be(exampleVideo.Trailer.FilePath);
        video.Trailer?.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
        video.Trailer?.Status.Should().Be(exampleVideo.Trailer.Status);
        video.Genres.Should().BeEquivalentTo(exampleVideo.Genres);
        video.Categories.Should().BeEquivalentTo(exampleVideo.Categories);
        video.CastMembers.Should().BeEquivalentTo(exampleVideo.CastMembers);
    }

    [Fact(DisplayName = nameof(Search))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task Search()
    {
        var exampleVideosList = this.fixture.GetExampleVideoList();
        var assertDbContext = fixture.CreateDbContext();
        await assertDbContext.Videos.AddRangeAsync(exampleVideosList);
        await assertDbContext.SaveChangesAsync();

        var actDbContext = fixture.CreateDbContext(true);
        var videoRepository = new VideoRepository(actDbContext);
        var searchInput = new SearchInput(1,20,"", "", default );
        var result =  await videoRepository.Search(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.Total.Should().Be(exampleVideosList.Count);
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Items.Count.Should().Be(exampleVideosList.Count);
        result.Items.Should().BeEquivalentTo(exampleVideosList);

    }

    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task SearchReturnsEmptyWhenEmpty()
    {
        var actDbContext = fixture.CreateDbContext(false);
        var videoRepository = new VideoRepository(actDbContext);
        var searchInput = new SearchInput(1,20,"", "", default );
        var result =  await videoRepository.Search(searchInput, CancellationToken.None);

        result.Should().NotBeNull();
        result.Total.Should().Be(0);
        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Items.Count.Should().Be(0);

    }

    [Theory(DisplayName = nameof(SearchReturnsPaginated))]
    [InlineData(10,1,5,5)]
    [InlineData(10,2,5,5)]
    [InlineData(7,2,5,2)]
    [InlineData(7,3,5,0)]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsPaginated(int quantityToGenerate,int page, int perPage, int expectedQuantityItems)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleVideoList = this.fixture.GetExampleVideoList(quantityToGenerate);
        await dbContext.Videos.AddRangeAsync(exampleVideoList);
        await dbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var videoRepository = new VideoRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage,"", "", SearchOrder.Asc);

        var searchResult = await videoRepository.Search(searchInput,  CancellationToken.None);

        searchResult.Should().NotBeNull();
        searchResult.Items.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(expectedQuantityItems);
        searchResult.Total.Should().Be(exampleVideoList.Count);
        searchResult.PerPage.Should().Be(perPage);
        searchResult.CurrentPage.Should().Be(searchInput.Page);

    }

     [Theory(DisplayName = nameof(SearchByTextTitle))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData("Action",1,5,1,1)]
    [InlineData("Horror",1,5,3,3)]
    [InlineData("Horror",2,5,0,3)]
    [InlineData("Sci-Fi",1,5,4,4)]
    [InlineData("Sci-Fi",1,2,2,4)]
    [InlineData("Sci-Fi",2,3,1,4)]
    [InlineData("Sci-Fi other",1,3,0,0)]
    [InlineData("Robots",1,5,2,2)]
    public async Task SearchByTextTitle(string search ,int page, int perPage, int expectedQuantityItemsReturned, int expectedQuantityTotalItems)
    {
        var dbContext = this.fixture.CreateDbContext();
        var videoListByTitles = this.fixture.GetExampleVideoListByTitles([
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-Fi IA",
            "Sci-Fi Space",
            "Sci-Fi Robots",
            "Sci-Fi Future",
        ]);

        await dbContext.Videos.AddRangeAsync(videoListByTitles);
        await dbContext.SaveChangesAsync();

        var actDbContext = this.fixture.CreateDbContext(true);

        var videoRepository = new VideoRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);
        var searchResult = await videoRepository.Search(searchInput,  CancellationToken.None);
        searchResult.Should().NotBeNull();
        searchResult.Items.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(expectedQuantityItemsReturned);
        searchResult.Total.Should().Be(expectedQuantityTotalItems);
        searchResult.PerPage.Should().Be(perPage);
        searchResult.CurrentPage.Should().Be(searchInput.Page);
    }

    [Theory(DisplayName = nameof(SearchOrdered))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    [InlineData("title","asc")]
    [InlineData("title","desc")]
    [InlineData("id","desc")]
    [InlineData("id","asc")]
    [InlineData("createdAt","asc")]
    [InlineData("createdAt","desc")]
    public async Task SearchOrdered(string orderBy, string order)
    {
        var dbContext = this.fixture.CreateDbContext();
        var exampleVideoList = this.fixture.GetExampleVideoList(10);
        await dbContext.AddRangeAsync(exampleVideoList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new VideoRepository(dbContext);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1,20, "", orderBy, searchOrder);


        var output = await repository.Search(searchInput, CancellationToken.None);
        var expectedOrderedList = this.fixture.CloneVideoListOrdered(exampleVideoList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleVideoList.Count);
        output.Items.Should().HaveCount(exampleVideoList.Count);
        output.Items.Should().BeEquivalentTo(expectedOrderedList, options => options.WithStrictOrdering());
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);

    }

    [Fact(DisplayName = nameof(SearchReturnsRelations))]
    [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
    public async Task SearchReturnsRelations()
    {
        var arrangeDbContext = this.fixture.CreateDbContext();
        var exampleVideosList = this.fixture.GetExampleVideoList();

        foreach (var exampleVideo in exampleVideosList)
        {
            var castMembers = this.fixture.GetRandomCastMemberList();
            var categories = this.fixture.GetRandomCategoryList();
            var gernes = this.fixture.GetRandomGenresList();

            castMembers.ForEach(castMember =>
            {
                exampleVideo.AddCastMember(castMember.Id);
                arrangeDbContext.VideosCastMembers.Add(new VideosCastMembers(castMember.Id, exampleVideo.Id));
            });

            categories.ForEach(category =>
            {
                exampleVideo.AddCategory(category.Id);
                arrangeDbContext.VideosCategories.Add(new VideosCategories(category.Id, exampleVideo.Id));
            });

            gernes.ForEach(genre =>
            {
                exampleVideo.AddGenre(genre.Id);
                arrangeDbContext.VideosGenres.Add(new VideosGenres(genre.Id, exampleVideo.Id));
            });
            await arrangeDbContext.Genres.AddRangeAsync(gernes);
            await arrangeDbContext.CastMembers.AddRangeAsync(castMembers);
            await arrangeDbContext.Categories.AddRangeAsync(categories);
            await arrangeDbContext.Videos.AddAsync(exampleVideo);
        }

        await arrangeDbContext.SaveChangesAsync();
        var actDbContext = this.fixture.CreateDbContext(true);

        var videoRepository = new VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20,"", "", SearchOrder.Asc);

        var searchResult = await videoRepository.Search(searchInput,  CancellationToken.None);


        searchResult.Should().NotBeNull();
        searchResult.Items.Should().NotBeNull();
        searchResult.Items.Should().HaveCount(exampleVideosList.Count);
        searchResult.Total.Should().Be(exampleVideosList.Count);
        searchResult.PerPage.Should().Be(searchInput.PerPage);
        searchResult.CurrentPage.Should().Be(searchInput.Page);
        searchResult.Items.Select(x=>x.Id).Should().BeEquivalentTo(exampleVideosList.Select(x=>x.Id));

        foreach (var resultItem in searchResult.Items)
        {
            var exampleVideo = exampleVideosList.Find(x=>x.Id == resultItem.Id);
            resultItem.Categories.Should().NotBeNull();
            resultItem.Categories.Should().HaveCount(exampleVideo.Categories.Count);
            resultItem.Categories.Should().BeEquivalentTo(exampleVideo.Categories);
            resultItem.Genres.Should().NotBeNull();
            resultItem.Genres.Should().HaveCount(exampleVideo.Genres.Count);
            resultItem.Genres.Should().BeEquivalentTo(exampleVideo.Genres);
            resultItem.CastMembers.Should().NotBeNull();
            resultItem.CastMembers.Should().HaveCount(exampleVideo.CastMembers.Count);
            resultItem.CastMembers.Should().BeEquivalentTo(exampleVideo.CastMembers);

        }

    }


}