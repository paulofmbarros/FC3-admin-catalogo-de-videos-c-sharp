// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.CreateGenre;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;
using Moq;

[Collection(nameof(CreateGenreTestFixture))]
public class CreateGenreTests
{
    private readonly CreateGenreTestFixture fixture;

    public CreateGenreTests(CreateGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateGenre()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var useCase = new CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = this.fixture.GetExampleInput();


        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Insert(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);


    }

    [Fact(DisplayName = nameof(CreateWithRelatatedCategories))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateWithRelatatedCategories()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var useCase = new CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = this.fixture.GetExampleInputWithCategories();
        categoryRepositoryMock.Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input.Categories);

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Insert(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().HaveCount(input.Categories?.Count ?? 0);
        input.Categories?.ForEach(x=>output.Categories.Should().Contain(x));
        output.CreatedAt.Should().NotBeSameDateAs(default);


    }

    [Fact(DisplayName = nameof(CreateThrowWhenRelatedCategoryNotFound))]
    [Trait("Application", "CreateGenre - Use Cases")]
    public async Task CreateThrowWhenRelatedCategoryNotFound()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var input = this.fixture.GetExampleInputWithCategories();
        var exampleGuid = input.Categories.LastOrDefault();
        categoryRepositoryMock.Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input.Categories.FindAll(x=>x != exampleGuid));

        var useCase = new CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);


        var action = async ()  => await useCase.Handle(input, CancellationToken.None);
        
        await action.Should().ThrowAsync<RelatedAggregateException>().WithMessage($"Related category Id (or ids) not found: {exampleGuid}");

        genreRepositoryMock.Verify(x=>x.Insert(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Never);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        categoryRepositoryMock.Verify(x=>x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);

    }
}