// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.ListGenres;

using System.Collections.ObjectModel;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.ListGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Moq;

[Collection(nameof(ListGenreTestFixture))]
public class ListGenresTest
{
    private readonly ListGenreTestFixture fixture;

    public ListGenresTest(ListGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }


    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task ListGenres()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();

        var exampleGenresList = this.fixture.GetExampleGenresList();

        var input = this.fixture.GetListGenresInput();
        var outputRepositorySearch = new SearchOutput<Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: exampleGenresList,
            total: new Random().Next(50,200));


        genreRepositoryMock
            .Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputRepositorySearch);

        var useCase = new ListGenres(genreRepositoryMock.Object);


        var output = await useCase.Handle(input, CancellationToken.None);


        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().NotBeNull();
        output.Items.Should().HaveCount(exampleGenresList.Count);

        foreach (var expectedId in output.Items.Select(x=>x.Id))
        {
            exampleGenresList.Should().Contain(x => x.Id == expectedId);
        }


        genreRepositoryMock.Verify(x=>x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.
            Verify(x=>x.Search(It.Is<SearchInput>(searchInput=>searchInput.Page == input.Page
                                                               && searchInput.PerPage == input.PerPage
                                                               && searchInput.Search == input.Search
                                                               && searchInput.Order == input.Direction
                                                               && searchInput.OrderBy == input.Sort),
                It.IsAny<CancellationToken>()), Times.Once);


    }

     [Fact(DisplayName = nameof(ListEmpty))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task ListEmpty()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();

        var input = this.fixture.GetListGenresInput();
        var outputRepositorySearch = new SearchOutput<Genre>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<Genre>(),
            total: new Random().Next(50,200));


        genreRepositoryMock
            .Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputRepositorySearch);

        var useCase = new ListGenres(genreRepositoryMock.Object);


        var output = await useCase.Handle(input, CancellationToken.None);


        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().NotBeNull();
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.Should().BeEquivalentTo(outputRepositorySearch.Items);

        genreRepositoryMock.Verify(x=>x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.
            Verify(x=>x.Search(It.Is<SearchInput>(searchInput=>searchInput.Page == input.Page
                                                               && searchInput.PerPage == input.PerPage
                                                               && searchInput.Search == input.Search
                                                               && searchInput.Order == input.Direction
                                                               && searchInput.OrderBy == input.Sort),

                It.IsAny<CancellationToken>()), Times.Once);


    }

    [Fact(DisplayName = nameof(ListUsingDefaultInputValues))]
    [Trait("Application", "GetGenre - Use Cases")]
    public async Task ListUsingDefaultInputValues()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();

        var outputRepositorySearch = new SearchOutput<Genre>(
            currentPage: 1,
            perPage: 15,
            items: new List<Genre>(),
            total: new Random().Next(50,200));


        genreRepositoryMock
            .Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputRepositorySearch);

        var useCase = new ListGenres(genreRepositoryMock.Object);


        var output = await useCase.Handle(new ListGenresInput(), CancellationToken.None);



        genreRepositoryMock.
            Verify(x=>x.Search(It.Is<SearchInput>(searchInput=>searchInput.Page == 1
                                                               && searchInput.PerPage == 15
                                                               && searchInput.Search == ""
                                                               && searchInput.Order == SearchOrder.Asc
                                                               && searchInput.OrderBy == ""),

                It.IsAny<CancellationToken>()), Times.Once);


    }
    
}