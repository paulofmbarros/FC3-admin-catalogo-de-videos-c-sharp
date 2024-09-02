// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.ListCategories;

using Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{

    private readonly ListCategoriesTestFixture fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCategories - Use Case")]
    public async Task List()
    {
        // Arrange
        var categoriesExample = this.fixture.GetExampleCategories();
        var repositoryMock = this.fixture.GetRepositoryMock();
        var input = this.fixture.GetExampleInput();

        var outputRepositorySearch = new SearchOutput<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExample,
            total: new Random().Next(50,200));

        repositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>( searchInput =>
                    searchInput.Page == input.Page &&
                    searchInput.PerPage == input.PerPage &&
                    searchInput.Search == input.Search &&
                    searchInput.OrderBy == input.Sort &&
                    searchInput.Order == input.Direction
                    ),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputRepositorySearch);


        var useCase = new ListCategories(repositoryMock.Object);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().NotBeNull();
        output.Items.Should().HaveCount(categoriesExample.Count);
        output.Items.Should().BeEquivalentTo(categoriesExample);

        repositoryMock.Verify(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [Theory(DisplayName = nameof(ListWithInputWithoutAllParameters))]
    [Trait("Application", "ListCategories - Use Case")]
    [MemberData(
        nameof(ListCategoriesTestDataGenerator.GetInputsWithoutAllParameters),
        parameters: 12,
        MemberType = typeof(ListCategoriesTestDataGenerator))]
    public async Task ListWithInputWithoutAllParameters(ListCategoriesInput input)
    {
        // Arrange
        var categoriesExample = this.fixture.GetExampleCategories();
        var repositoryMock = this.fixture.GetRepositoryMock();

        var outputRepositorySearch = new SearchOutput<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: categoriesExample,
            total: new Random().Next(50,200));

        repositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>( searchInput =>
                    searchInput.Page == input.Page &&
                    searchInput.PerPage == input.PerPage &&
                    searchInput.Search == input.Search &&
                    searchInput.OrderBy == input.Sort &&
                    searchInput.Order == input.Direction
                ),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputRepositorySearch);


        var useCase = new ListCategories(repositoryMock.Object);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().NotBeNull();
        output.Items.Should().HaveCount(categoriesExample.Count);
        output.Items.Should().BeEquivalentTo(categoriesExample);

        repositoryMock.Verify(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact(DisplayName = nameof(ListOkWhenEmpty))]
    [Trait("Application", "ListCategories - Use Case")]
    public async Task ListOkWhenEmpty()
    {
        // Arrange
        var repositoryMock = this.fixture.GetRepositoryMock();
        var input = this.fixture.GetExampleInput();

        var outputRepositorySearch = new SearchOutput<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: (new List<Category>()).AsReadOnly(),
            total: 0);

        repositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>( searchInput =>
                    searchInput.Page == input.Page &&
                    searchInput.PerPage == input.PerPage &&
                    searchInput.Search == input.Search &&
                    searchInput.OrderBy == input.Sort &&
                    searchInput.Order == input.Direction
                ),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputRepositorySearch);


        var useCase = new ListCategories(repositoryMock.Object);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().NotBeNull();
        output.Items.Should().HaveCount(0);


        repositoryMock.Verify(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()), Times.Once);

    }

}