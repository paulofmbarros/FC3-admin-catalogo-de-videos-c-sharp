// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.CreateVideo;

using System.Text;
using Fc.CodeFlix.Catalog.Application.Common;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.Common;
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
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var storageServiceMock = new Mock<IStorageService>();


        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        var input = this.fixture.CreateValidInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened
        ), It.IsAny<CancellationToken>()));

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

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
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var storageServiceMock = new Mock<IStorageService>();


        var useCase = new CreateVideo(videoRepositoryMock.Object, categoryRepositoryMock.Object,
            genreRepositoryMock.Object, castMemberRepositoryMock.Object, unitOfWorkMock.Object, storageServiceMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>();

        exceptionAssertion
            .WithMessage($"There are validation errors")
            .Which.Errors!.FirstOrDefault().Message.Should().Be(expectedValidationError);

        videoRepositoryMock.Verify(x => x.Insert(It.IsAny<Video>(), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithCategoriesIds()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var storageServiceMock = new Mock<IStorageService>();


        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        categoryRepositoryMock
            .Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds.AsReadOnly());

        var useCase = new CreateVideo(videoRepositoryMock.Object, categoryRepositoryMock.Object,
            genreRepositoryMock.Object, castMemberRepositoryMock.Object, unitOfWorkMock.Object,storageServiceMock.Object);


        var input = this.fixture.CreateValidInput(exampleCategoriesIds);

        var output = await useCase.Handle(input, CancellationToken.None);


        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEquivalentTo(exampleCategoriesIds);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
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

        categoryRepositoryMock.Verify(
            x => x.GetIdsByIds(It.Is<List<Guid>>(ids => ids.Count == exampleCategoriesIds.Count),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(ThrowsWhenInvalidCategoriesIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenInvalidCategoriesIds()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var storageServiceMock = new Mock<IStorageService>();

        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedCategoryId = exampleCategoriesIds[2];
        categoryRepositoryMock
            .Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds.FindAll(x => x != removedCategoryId).AsReadOnly());

        var useCase = new CreateVideo(videoRepositoryMock.Object, categoryRepositoryMock.Object,
            genreRepositoryMock.Object, castMemberRepositoryMock.Object, unitOfWorkMock.Object, storageServiceMock.Object);
        var input = this.fixture.CreateValidInput(exampleCategoriesIds);

        var action = async () => await useCase.Handle(input, CancellationToken.None);
        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related Category id (or ids) not found {removedCategoryId}.");
    }

    [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithGenresIds()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var storageServiceMock = new Mock<IStorageService>();

        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

        genreRepositoryMock
            .Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var useCase = new CreateVideo(videoRepositoryMock.Object, categoryRepositoryMock.Object,
            genreRepositoryMock.Object, castMemberRepositoryMock.Object, unitOfWorkMock.Object, storageServiceMock.Object);


        var input = this.fixture.CreateValidInput(genresIds: exampleIds);

        var output = await useCase.Handle(input, CancellationToken.None);


        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEquivalentTo(exampleIds);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened &&
            video.Genres.All(id => exampleIds.Contains(id))
        ), It.IsAny<CancellationToken>()));

        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(ThrowsWhenGenreInvalidGenreId))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenGenreInvalidGenreId()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var storageServiceMock = new Mock<IStorageService>();

        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedGenreId = exampleIds[2];
        genreRepositoryMock
            .Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.FindAll(x => x != removedGenreId).AsReadOnly());

        var useCase = new CreateVideo(videoRepositoryMock.Object, categoryRepositoryMock.Object,
            genreRepositoryMock.Object, castMemberRepositoryMock.Object, unitOfWorkMock.Object, storageServiceMock.Object);

        var input = this.fixture.CreateValidInput(genresIds: exampleIds);

        var action = () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related Genre id (or ids) not found {removedGenreId}.");

        genreRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithCastMembersIds))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithCastMembersIds()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
        var storageServiceMock = new Mock<IStorageService>();

        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

        castMembersRepositoryMock
            .Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var useCase = new CreateVideo(videoRepositoryMock.Object, categoryRepositoryMock.Object,
            genreRepositoryMock.Object, castMembersRepositoryMock.Object, unitOfWorkMock.Object, storageServiceMock.Object);


        var input = this.fixture.CreateValidInput(castMembersIds: exampleIds);

        var output = await useCase.Handle(input, CancellationToken.None);


        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CategoriesIds.Should().BeEmpty();
        output.GenresIds.Should().BeEmpty();
        output.CastMembersIds.Should().BeEquivalentTo(exampleIds);

        videoRepositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened &&
            video.Genres.All(id => exampleIds.Contains(id))
        ), It.IsAny<CancellationToken>()));

        castMembersRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithThumb))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithThumb()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var expectedThumbName = $"thumb.jpg";


        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbName);

        var input = this.fixture.CreateValidInput(thumb: this.fixture.GetImageValidFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened
        ), It.IsAny<CancellationToken>()));

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.VerifyAll();

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Thumb.Should().Be(expectedThumbName);
    }

     [Fact(DisplayName = nameof(CreateVideoWithBanner))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithBanner()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var expectedBannerName = $"banner.jpg";


        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBannerName);

        var input = this.fixture.CreateValidInput(banner: this.fixture.GetImageValidFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened
        ), It.IsAny<CancellationToken>()));

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.VerifyAll();

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Banner.Should().Be(expectedBannerName);
    }

     [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithThumbHalf()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var expectedThumbHalfName = $"thumbHalf.jpg";


        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbHalfName);

        var input = this.fixture.CreateValidInput(thumbHalf: this.fixture.GetImageValidFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened
        ), It.IsAny<CancellationToken>()));

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.VerifyAll();

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalf.Should().Be(expectedThumbHalfName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithAllImages))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithAllImages()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var expectedThumbHalfName = $"thumbHalf.jpg";
        var expectedThumbName = $"thumb.jpg";
        var expectedBannerName = $"banner.jpg";


        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith("-banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBannerName);

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith("-thumbhalf.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbHalfName);

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith("-thumb.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbName);

        var input = this.fixture.CreateValidInputWithAllImages();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened
        ), It.IsAny<CancellationToken>()));

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.VerifyAll();

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalf.Should().Be(expectedThumbHalfName);
        output.Banner.Should().Be(expectedBannerName);
        output.Thumb.Should().Be(expectedThumbName);
    }

     [Fact(DisplayName = nameof(ThrowsExceptionInUploadErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionInUploadErrorCases()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();

        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith("-banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong in uplaod"));

        var input = this.fixture.CreateValidInputWithAllImages();

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong in uplaod");


    }

    [Fact(DisplayName = nameof(ThrowsExceptionInAndRollbackInImagesUploadErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionInAndRollbackInImagesUploadErrorCases()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var expectedThumbName = $"thumb.jpg";
        var expectedBannerName = $"banner.jpg";


        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith("-banner.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBannerName);

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith("-thumbhalf.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Something went wrong in uplaod"));

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith("-thumb.jpg")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedThumbName);

        var input = this.fixture.CreateValidInputWithAllImages();

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong in uplaod");

        storageServiceMock.Verify(x=> x.Delete(It.Is<string>(x=> x.EndsWith("banner.jpg") | x.EndsWith("thumb.jpg") ), It.IsAny<CancellationToken>()), Times.Exactly(2));


    }

    [Fact(DisplayName = nameof(ThrowsWhenCastWhenInvalidCastMemberId))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsWhenCastWhenInvalidCastMemberId()
    {
        var videoRepositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var castMembersRepositoryMock = new Mock<ICastMemberRepository>();
        var storageServiceMock = new Mock<IStorageService>();

        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var removedCastMemberId = exampleIds[2];

        castMembersRepositoryMock
            .Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds.FindAll(x=> x != removedCastMemberId));

        var useCase = new CreateVideo(videoRepositoryMock.Object, categoryRepositoryMock.Object,
            genreRepositoryMock.Object, castMembersRepositoryMock.Object, unitOfWorkMock.Object, storageServiceMock.Object);


        var input = this.fixture.CreateValidInput(castMembersIds: exampleIds);

        var action= () => useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related CastMember id (or ids) not found {removedCastMemberId}.");

        castMembersRepositoryMock.VerifyAll();
    }

    [Fact(DisplayName = nameof(CreateVideoWithMedia))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithMedia()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var expectedMediaName = $"/storage/{this.fixture.GetValidMediaPath()}";


        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMediaName);

        var input = this.fixture.CreateValidInput(media: this.fixture.GetMediaValidFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened
        ), It.IsAny<CancellationToken>()));

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.VerifyAll();

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Media.Should().Be(expectedMediaName);
    }

    [Fact(DisplayName = nameof(CreateVideoWithTrailer))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task CreateVideoWithTrailer()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var expectedTrailerName = $"/storage/{this.fixture.GetValidMediaPath()}";


        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTrailerName);

        var input = this.fixture.CreateValidInput(trailer: this.fixture.GetMediaValidFileInput());

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(x => x.Insert(It.Is<Video>(video =>
            video.Title == input.Title &&
            video.Published == input.Published &&
            video.Description == input.Description &&
            video.Duration == input.Duration &&
            video.Rating == input.Rating &&
            video.Id != Guid.Empty &&
            video.YearLaunched == input.YearLaunched &&
            video.Opened == input.Opened
        ), It.IsAny<CancellationToken>()));

        unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        storageServiceMock.VerifyAll();

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Trailer.Should().Be(expectedTrailerName);
    }

    [Fact(DisplayName = nameof(ThrowsExceptionInAndRollbackInMediaUploadCommitErrorCases))]
    [Trait("Application", "CreateVideo - Use Cases")]
    public async Task ThrowsExceptionInAndRollbackInMediaUploadCommitErrorCases()
    {
        var repositoryMock = new Mock<IVideoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var categoryRepositoryMock = new Mock<ICategoryRepository>();
        var genreRepositoryMock = new Mock<IGenreRepository>();
        var storageServiceMock = new Mock<IStorageService>();
        var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
        var storageMediaPath = this.fixture.GetValidMediaPath();
        var storageTrailerPath = this.fixture.GetValidMediaPath();
        var storagePathList = new List<string>() { storageMediaPath, storageTrailerPath };

        var input = this.fixture.CreateValidInputWithAllMedias();

        var useCase = new CreateVideo(repositoryMock.Object, categoryRepositoryMock.Object, genreRepositoryMock.Object, castMemberRepositoryMock.Object,
            unitOfWorkMock.Object, storageServiceMock.Object);

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith($"media.{input.Media.Extension}")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(storageMediaPath);

        storageServiceMock.Setup(x=> x.Upload(It.Is<string>(x=> x.EndsWith($"trailer.{input.Trailer.Extension}")), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(storageTrailerPath);

        unitOfWorkMock.Setup(x => x.Commit(It.IsAny<CancellationToken>())).Throws(new Exception("Something went wrong with the commit"));

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong with the commit");

        storageServiceMock.Verify(x=> x.Delete(It.Is<string>(path => storagePathList.Contains(path) ), It.IsAny<CancellationToken>()), Times.Exactly(2));
        storageServiceMock.Verify(x=> x.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));


    }

}