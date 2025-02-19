// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.CreateVideo;

using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Fc.CodeFlix.Catalog.Domain.Validation;
using FluentAssertions;
using Moq;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{

    private readonly CreateVideoTestFixture fixture;

    public CreateVideoTest(CreateVideoTestFixture fixture) => this.fixture = fixture;

    [Fact(DisplayName = nameof(CreateVideo))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideo()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var useCase = new CreateVideo(repositoryMock.Object, unitOfWorkMock.Object);

        var input = this.fixture.CreateValidCreateVideoInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x=>x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened
            ), It.IsAny<CancellationToken>()));

        unitOfWorkMock.Verify(x=> x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);


    }

    [Theory(DisplayName = nameof(CreateVideoThrowsWithInvalidInput))]
    [Trait("Application", "CreateVideo - Use Cases")]
    [ClassData(typeof(CreateVideoTestDataGenerator))]
    public async Task CreateVideoThrowsWithInvalidInput(CreateVideoInput input, string expectedValidationError)
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var useCase = new CreateVideo(repositoryMock.Object, unitOfWorkMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>();

        exceptionAssertion
            .WithMessage($"There are validation errors")
            .Which.Errors!.FirstOrDefault().Message.Should().Be(expectedValidationError);

        repositoryMock.Verify(x => x.Insert(It.IsAny<Video>(), It.IsAny<CancellationToken>()), Times.Never());

    }

    [Fact(DisplayName = nameof(CreateVideo))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithCategoriesIds()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var useCase = new CreateVideo(repositoryMock.Object, unitOfWorkMock.Object);

        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var input = this.fixture.CreateValidCreateVideoInput(exampleCategoriesIds);

        var output = await useCase.Handle(input, CancellationToken.None);



        unitOfWorkMock.Verify(x=> x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoryIds.Should().Be().EquivalentTo(exampleCategoriesIds);

        repositoryMock.Verify(x=>x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened &&
            video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId))
        ), It.IsAny<CancellationToken>()));


    }

}