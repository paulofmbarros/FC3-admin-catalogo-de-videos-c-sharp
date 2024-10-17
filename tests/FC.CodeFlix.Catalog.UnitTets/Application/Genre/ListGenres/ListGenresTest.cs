// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.ListGenres;

using Fc.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
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

        var input = this.fixture.GetExampleInput();
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
        output.Items.Should().BeEquivalentTo(exampleGenresList);

        foreach (var expectedId in output.Categories)
        {
            exampleGenresList.Categories.Should().Contain(x => x == expectedId);
        }


        genreRepositoryMock.Verify(x=>x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()), Times.Once);


    }
    
}