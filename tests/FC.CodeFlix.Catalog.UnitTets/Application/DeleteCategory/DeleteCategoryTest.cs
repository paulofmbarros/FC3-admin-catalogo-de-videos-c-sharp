// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.DeleteCategory;

using Fc.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using Fc.CodeFlix.Catalog.Domain.Entity;
using FluentAssertions;
using MediatR;
using Moq;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Application ", "Delete Category - Use Case")]
    public async Task DeleteCategory()
    {
        // Arrange

        var repositoryMock = fixture.GetRepositoryMock();
        var unitOfWorkMock = fixture.GetUnitOfWorkMock();
        var categoryExample = fixture.GetValidCategory();

        repositoryMock.Setup(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryExample);

        var input = new DeleteCategoryInput(categoryExample.Id);
        var useCase = new DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);

        // Act
        var result = await useCase.Handle(input, CancellationToken.None);

        // Assert
        repositoryMock.Verify(x=> x.Get(categoryExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Delete(categoryExample, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application ", "Delete Category - Use Case")]
    public async Task ThrowWhenCategoryNotFound()
    {
        // Arrange

        var repositoryMock = this.fixture.GetRepositoryMock();
        var unitOfWorkMock = this.fixture.GetUnitOfWorkMock();
        var categoryExample = this.fixture.GetValidCategory();

        repositoryMock.Setup(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Category not found"));

        var input = new DeleteCategoryInput(categoryExample.Id);
        var useCase = new DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);

        // Act
        var task = async() => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await task.Should().ThrowAsync<Exception>();

        repositoryMock.Verify(x=> x.Get(categoryExample.Id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.Delete(categoryExample, It.IsAny<CancellationToken>()), Times.Never);
        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }


}