// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.GetCategory;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FluentAssertions;
using Moq;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Application ", "GetCategory - Use Cases")]
    public async Task GetCategory()
    {
        // Arrange
        var repositoryMock = this.fixture.GetRepositoryMock();
        var exampleCategory = this.fixture.GetValidCategory();
        repositoryMock.Setup(x=>x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategory);
        var input = new GetCategoryInput(exampleCategory.Id);
        var useCase = new GetCategory(repositoryMock.Object);
        // Act

        var output = await useCase.Handle(input, CancellationToken.None);



        // Assert
        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(exampleCategory.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be(exampleCategory.IsActive);
        output.CreatedAt.Should().Be(exampleCategory.CreatedAt);
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesNotExist))]
    [Trait("Application ", "GetCategory - Use Cases")]
    public async Task NotFoundExceptionWhenCategoryDoesNotExist()
    {
        // Arrange
        var repositoryMock = this.fixture.GetRepositoryMock();
        var exampleCategoryId = Guid.NewGuid();
        repositoryMock.Setup(x=>x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new NotFoundException($"Category with id {exampleCategoryId} not found")
                );

        var input = new GetCategoryInput(exampleCategoryId);
        var useCase = new GetCategory(repositoryMock.Object);
        // Act

        var task = async() => await useCase.Handle(input, CancellationToken.None);

        // Assert
        await task.Should().ThrowAsync<NotFoundException>();

    }
    
}