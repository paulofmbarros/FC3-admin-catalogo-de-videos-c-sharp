// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.ListVideos;

using Fc.CodeFlix.Catalog.Application.UseCases.Video.ListVideo;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;

[Collection(nameof(ListVideosTestFixture))]
public class ListVideosTest
{
    private readonly ListVideosTestFixture fixture;
    private readonly Mock<IVideoRepository> videoRepositoryMock;
    private readonly ListVideos useCase;

    public ListVideosTest(ListVideosTestFixture fixture)
    {
        this.fixture = fixture;
        this.videoRepositoryMock = new Mock<IVideoRepository>();
        this.useCase = new ListVideos(this.videoRepositoryMock.Object);
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
        });
        this.videoRepositoryMock.VerifyAll();
    }

}