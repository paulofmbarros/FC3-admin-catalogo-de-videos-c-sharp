// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.ListVideos;

using Fc.CodeFlix.Catalog.Application.UseCases.Video.ListVideo;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Extensions;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;

[Collection(nameof(ListVideosTestFixture))]
public class ListVideosTest
{
    private readonly ListVideosTestFixture fixture;
    private readonly Mock<IVideoRepository> videoRepositoryMock;
    private readonly Mock<ICategoryRepository> categoryRepositoryMock;
    private readonly Mock<IGenreRepository> genreRepositoryMock;
    private readonly ListVideos useCase;

    public ListVideosTest(ListVideosTestFixture fixture)
    {
        this.fixture = fixture;
        this.videoRepositoryMock = new Mock<IVideoRepository>();
        this.categoryRepositoryMock = new Mock<ICategoryRepository>();
        this.genreRepositoryMock = new Mock<IGenreRepository>();
        this.useCase = new ListVideos(this.videoRepositoryMock.Object, this.categoryRepositoryMock.Object, this.genreRepositoryMock.Object);
    }

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task List()
    {
        var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var exampleVideosList = this.fixture.CreateExampleVideosList();
        this.videoRepositoryMock.Setup(x=> x.Search(It.Is<SearchInput>(x=>
                x.Page == input.Page &&
                x.PerPage == input.PerPage &&
                x.Search == input.Search &&
                x.OrderBy == input.Sort &&
                x.Order == input.Direction), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchOutput<Video>(input.Page,input.PerPage, exampleVideosList.Count, exampleVideosList));

        var output = await this.useCase.Handle(input, new CancellationToken());

        //Assert
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.ToList().ForEach(output =>
        {
            var exampleVideo = exampleVideosList.Find(x => x.Id == output.Id);
            exampleVideo.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBe(default);
            output.Title.Should().Be(exampleVideo.Title);
            output.Published.Should().Be(exampleVideo.Published);
            output.Description.Should().Be(exampleVideo.Description);
            output.Duration.Should().Be(exampleVideo.Duration);
            output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
            output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            output.Opened.Should().Be(exampleVideo.Opened);
            output.ThumbFileUrl.Should().Be(exampleVideo.Thumb.Path);
            output.ThumbHalf.Should().Be(exampleVideo.ThumbHalf.Path);
            output.BannerFileUrl.Should().Be(exampleVideo.Banner.Path);
            output.MediaFileUrl.Should().Be(exampleVideo.Media.FilePath);
            output.TrailerFileUrl.Should().Be(exampleVideo.Trailer.FilePath);
            var outputItemsCategoriesIds = output.Categories.Select(dto => dto.Id).ToList();
            outputItemsCategoriesIds.Should().BeEquivalentTo(exampleVideo.Categories);
            var outputItemsGenresIds = output.Genres.Select(dto => dto.Id).ToList();
            outputItemsGenresIds.Should().BeEquivalentTo(exampleVideo.Genres);
            var outputItemsCastMembersIds = output.CastMembers.Select(dto => dto.Id).ToList();
            outputItemsCastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);
        });
        this.videoRepositoryMock.VerifyAll();
    }


    [Fact(DisplayName = nameof(ListReturnsEmptyWhenThereIsNoVideos))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task ListReturnsEmptyWhenThereIsNoVideos()
    {
        var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var exampleVideosList = new List<Video>();
        this.videoRepositoryMock.Setup(x=> x.Search(It.Is<SearchInput>(x=>
                x.Page == input.Page &&
                x.PerPage == input.PerPage &&
                x.Search == input.Search &&
                x.OrderBy == input.Sort &&
                x.Order == input.Direction), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchOutput<Video>(input.Page,input.PerPage, exampleVideosList.Count, exampleVideosList));

        var output = await this.useCase.Handle(input, new CancellationToken());

        //Assert
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
        this.videoRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ListVideosWithRelations))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task ListVideosWithRelations()
    {
        var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var (exampleVideosList, exampleCategories, exampleGenres) = this.fixture.CreateExampleVideosListWithRelations();
        var examplesCategoriesIds = exampleCategories.Select(category => category.Id).ToList();
        var exampleGenresIds = exampleGenres.Select(genre => genre.Id).ToList();

        this.categoryRepositoryMock
            .Setup(x=> x.GetListByIds(It.Is<List<Guid>>(list => list.All(examplesCategoriesIds.Contains) && list.Count == examplesCategoriesIds.Count), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategories);

        this.genreRepositoryMock
            .Setup(x=> x.GetListByIds(It.Is<List<Guid>>(list => list.All(exampleGenresIds.Contains) && list.Count == exampleGenresIds.Count), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenres);

        this.videoRepositoryMock.Setup(x=> x.Search(It.Is<SearchInput>(x=>
                x.Page == input.Page &&
                x.PerPage == input.PerPage &&
                x.Search == input.Search &&
                x.OrderBy == input.Sort &&
                x.Order == input.Direction), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchOutput<Video>(input.Page,input.PerPage, exampleVideosList.Count, exampleVideosList));

        var output = await this.useCase.Handle(input, new CancellationToken());

        //Assert
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideosList.Count);
        output.Items.Should().HaveCount(exampleVideosList.Count);
        output.Items.ToList().ForEach(output =>
        {
            var exampleVideo = exampleVideosList.Find(x => x.Id == output.Id);
            exampleVideo.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBe(default);
            output.Title.Should().Be(exampleVideo.Title);
            output.Published.Should().Be(exampleVideo.Published);
            output.Description.Should().Be(exampleVideo.Description);
            output.Duration.Should().Be(exampleVideo.Duration);
            output.Rating.Should().Be(exampleVideo.Rating.ToStringSignal());
            output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            output.Opened.Should().Be(exampleVideo.Opened);
            output.ThumbFileUrl.Should().Be(exampleVideo.Thumb.Path);
            output.ThumbHalf.Should().Be(exampleVideo.ThumbHalf.Path);
            output.BannerFileUrl.Should().Be(exampleVideo.Banner.Path);
            output.MediaFileUrl.Should().Be(exampleVideo.Media.FilePath);
            output.TrailerFileUrl.Should().Be(exampleVideo.Trailer.FilePath);
            output.Categories.ToList().ForEach(relation =>
            {
                var exampleCategory = exampleCategories.Find(x => x.Id == relation.Id);
                exampleCategory.Should().NotBeNull();
                relation.Name.Should().Be(exampleCategory?.Name);

            });
            output.Genres.ToList().ForEach(relation =>
            {
                var exampleGenre = exampleGenres.Find(x => x.Id == relation.Id);
                exampleGenre.Should().NotBeNull();
                relation.Name.Should().Be(exampleGenre?.Name);

            });
            var outputItemsCastMembersIds = output.CastMembers.Select(dto => dto.Id).ToList();
            outputItemsCastMembersIds.Should().BeEquivalentTo(exampleVideo.CastMembers);

        });
        this.videoRepositoryMock.VerifyAll();
        this.genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ListVideosWithoutRelationsDoesntCallOtherRepositories))]
    [Trait("Application", "ListVideos - Use Cases")]
    public async Task ListVideosWithoutRelationsDoesntCallOtherRepositories()
    {
        var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);
        var exampleVideos = this.fixture.CreateExampleVideosListWithoutRelations();

        this.videoRepositoryMock.Setup(x=> x.Search(It.Is<SearchInput>(x=>
                x.Page == input.Page &&
                x.PerPage == input.PerPage &&
                x.Search == input.Search &&
                x.OrderBy == input.Sort &&
                x.Order == input.Direction), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchOutput<Video>(input.Page,input.PerPage, exampleVideos.Count, exampleVideos));

        var output = await this.useCase.Handle(input, new CancellationToken());

        //Assert
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(exampleVideos.Count);
        output.Items.Should().HaveCount(exampleVideos.Count);
        output.Items.ToList().ForEach(output =>
        {
            var exampleVideo = exampleVideos.Find(x => x.Id == output.Id);
            exampleVideo.Should().NotBeNull();
            output.Should().NotBeNull();
            output.Categories.Should().HaveCount(0);
            output.Genres.Should().HaveCount(0);

        });
        this.videoRepositoryMock.VerifyAll();
        this.categoryRepositoryMock.Verify(x=> x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        this.genreRepositoryMock.Verify(x=> x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);

    }
}