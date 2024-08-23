// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.UpdateCategory;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task UpdateCategory(Category exampleCategory, UpdateCategoryInput input )
    {
        // Arrange
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();


        repositoryMock
            .Setup(x => x
                .Get(exampleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);

        var useCase = new UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        Assert.NotNull(output);
        output.Should().BeOfType<CategoryModelOutput>();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        repositoryMock.Verify(x => x.Get(exampleCategory.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound) )]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        // Arrange
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var input = this.fixture.GenerateUpdateCategoryInput();

        repositoryMock
            .Setup(x => x
                .Get(input.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync( new NotFoundException($"Category {input.Id} not found"));

        var useCase = new UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        // Act
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await task.Should().ThrowAsync<NotFoundException>();
        repositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);


    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutProvidingIsActive))]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task UpdateCategoryWithoutProvidingIsActive(Category exampleCategory, UpdateCategoryInput exampleInput )
    {
        // Arrange
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var input = new UpdateCategoryInput(exampleCategory.Id, exampleCategory.Name, exampleCategory.Description);
        repositoryMock
            .Setup(x => x
                .Get(exampleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);

        var useCase = new UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        Assert.NotNull(output);
        output.Should().BeOfType<CategoryModelOutput>();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        repositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

    }

    [Theory(DisplayName = nameof(UpdateCategoryWithOnlyName))]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task UpdateCategoryWithOnlyName(Category exampleCategory, UpdateCategoryInput exampleInput )
    {
        // Arrange
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();

        var input = new UpdateCategoryInput(exampleCategory.Id, exampleCategory.Name);
        repositoryMock
            .Setup(x => x
                .Get(exampleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);

        var useCase = new UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        // Act
        var output = await useCase.Handle(input, CancellationToken.None);

        // Assert
        Assert.NotNull(output);
        output.Should().BeOfType<CategoryModelOutput>();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        repositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsShorterThanThreeCharacters))]
    [Trait("Application ", "UpdateCategory - Use Cases")]
    [MemberData(nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 12,
        MemberType = typeof(UpdateCategoryTestDataGenerator))]
    public async Task ThrowWhenNameIsShorterThanThreeCharacters(UpdateCategoryInput input, string expectedExceptionMessage)
    {
        // Arrange
        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var exampleCategory = this.fixture.GetValidCategory();

        repositoryMock
            .Setup(x => x
                .Get(input.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);

        var useCase = new UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        // Act
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);
        repositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Update(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);


    }
}