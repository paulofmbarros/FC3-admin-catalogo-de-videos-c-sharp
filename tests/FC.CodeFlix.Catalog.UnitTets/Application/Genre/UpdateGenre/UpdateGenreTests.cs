// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Genre.UpdateGenre;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.UpdateGenre;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;

[Collection(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTests
{
    private readonly UpdateGenreTestFixture fixture;

    public UpdateGenreTests(UpdateGenreTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenre()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleGenre = this.fixture.GetExampleGenre();
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);


        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id ,this.fixture.GetValidGenreName(), this.fixture.GetRandomBoolean());

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Update(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(0);

        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x=>x.Update(It.Is<Genre>(g=>g.Id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);


    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var exampleId = Guid.NewGuid();

        genreRepositoryMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Genre {exampleId} not found."));


        var useCase = new UpdateGenre(genreRepositoryMock.Object, this.fixture.GetUnitOfWorkMock().Object, this.fixture.GetCategoryRepositoryMock().Object);
        var input = new UpdateGenreInput(exampleId , this.fixture.GetValidGenreName(), this.fixture.GetRandomBoolean());

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage($"Genre {exampleId} not found.");

    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ThrowWhenNameIsInvalid(string? name)
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleGenre = this.fixture.GetExampleGenre();
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);


        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id ,name, this.fixture.GetRandomBoolean());

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>().WithMessage("Name should not be empty or null.");

    }

    [Theory(DisplayName = nameof(UpdateGenreOnlyName))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateGenreOnlyName(bool isActive)
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleGenre = this.fixture.GetExampleGenre(isActive);
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);


        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id ,this.fixture.GetValidGenreName());

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Update(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(isActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(0);

        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x=>x.Update(It.Is<Genre>(g=>g.Id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);


    }

    [Fact(DisplayName = nameof(UpdateGenreAddingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreAddingCategoriesIds()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleGenre = this.fixture.GetExampleGenre();
        var exampleCategories = this.fixture.GetRandomIdsList();
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        categoryRepositoryMock.Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategories);

        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id,
            this.fixture.GetValidGenreName(),
            this.fixture.GetRandomBoolean(),
            exampleCategories);

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Update(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(exampleCategories.Count);
        exampleCategories.ForEach(expectedId=>output.Categories.Should().Contain(expectedId));

        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x=>x.Update(It.Is<Genre>(g=>g.Id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);


    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleGenre = this.fixture.GetExampleGenre(categoriesIds: this.fixture.GetRandomIdsList());
        var exampleNewCategories = this.fixture.GetRandomIdsList(10);
        var listReturnedByCategoryRepository = exampleNewCategories.GetRange(0, exampleNewCategories.Count - 2);

        var idsNotFound = exampleNewCategories.Except(listReturnedByCategoryRepository).ToList();

        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        categoryRepositoryMock.Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(listReturnedByCategoryRepository);


        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id,
            this.fixture.GetValidGenreName(),
            this.fixture.GetRandomBoolean(),
            exampleNewCategories);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage("Related category Id (or ids) not found: " + string.Join(", ", idsNotFound));

    }

      [Fact(DisplayName = nameof(UpdateGenreReplacingCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreReplacingCategoriesIds()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleGenre = this.fixture.GetExampleGenre(categoriesIds: this.fixture.GetRandomIdsList());
        var exampleCategories = this.fixture.GetRandomIdsList();
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);


        categoryRepositoryMock.Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategories);

        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id,
            this.fixture.GetValidGenreName(),
            this.fixture.GetRandomBoolean(),
            exampleCategories);

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Update(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(exampleCategories.Count);
        exampleCategories.ForEach(expectedId=>output.Categories.Should().Contain(expectedId));

        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x=>x.Update(It.Is<Genre>(g=>g.Id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact(DisplayName = nameof(UpdateGenreWithoutCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithoutCategoriesIds()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleCategories = this.fixture.GetRandomIdsList();

        var exampleGenre = this.fixture.GetExampleGenre(categoriesIds: exampleCategories);
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id,
            this.fixture.GetValidGenreName(),
            this.fixture.GetRandomBoolean());

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Update(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(exampleCategories.Count);
        exampleCategories.ForEach(expectedId=>output.Categories.Should().Contain(expectedId));

        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x=>x.Update(It.Is<Genre>(g=>g.Id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact(DisplayName = nameof(UpdateGenreWithEmptyCategoriesIds))]
    [Trait("Application", "UpdateGenre - Use Cases")]
    public async Task UpdateGenreWithEmptyCategoriesIds()
    {
        var genreRepositoryMock = this.fixture.GetGenreRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = this.fixture.GetCategoryRepositoryMock();
        var exampleCategories = this.fixture.GetRandomIdsList();

        var exampleGenre = this.fixture.GetExampleGenre(categoriesIds: exampleCategories);
        genreRepositoryMock
            .Setup(x => x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenre);

        var useCase = new UpdateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object, categoryRepositoryMock.Object);
        var input = new UpdateGenreInput(exampleGenre.Id,
            this.fixture.GetValidGenreName(),
            this.fixture.GetRandomBoolean(),
            new List<Guid>());

        var output = await useCase.Handle(input, CancellationToken.None);

        genreRepositoryMock.Verify(x=>x.Update(It.IsAny<Genre>(), It.IsAny<CancellationToken>() ), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Id.Should().NotBe(Guid.Empty);
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be((bool)input.IsActive);
        output.Categories.Should().NotBeNull();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        output.Categories.Should().HaveCount(0);

        genreRepositoryMock.Verify(x=>x.Get(It.Is<Guid>(id=>id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        genreRepositoryMock.Verify(x=>x.Update(It.Is<Genre>(g=>g.Id == exampleGenre.Id), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x=>x.Commit(It.IsAny<CancellationToken>()), Times.Once);

    }


}