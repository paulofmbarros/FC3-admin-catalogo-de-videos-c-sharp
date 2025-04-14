// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Video.UpdateVideo;

using Fc.CodeFlix.Catalog.Application.Common;
using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Extensions;
using Fc.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;

[Collection(nameof(UpdateVideosTestFixture))]
public class UpdateVideoTest
{
    private readonly UpdateVideosTestFixture fixture;
    private readonly Mock<IVideoRepository> videoRepository;
    private readonly Mock<IGenreRepository> genreRepository;
    private readonly Mock<ICategoryRepository> categoryRepository;
    private readonly Mock<ICastMemberRepository> castMemberRepository;
    private readonly Mock<IStorageService> storageService;
    private readonly Mock<IUnitOfWork> unitOfWork;
    private readonly UpdateVideo useCase;

    public UpdateVideoTest(UpdateVideosTestFixture fixture)
    {
        this.fixture = fixture;
        this.videoRepository = new Mock<IVideoRepository>();
        this.genreRepository = new Mock<IGenreRepository>();
        this.categoryRepository = new Mock<ICategoryRepository>();
        this.castMemberRepository = new Mock<ICastMemberRepository>();
        this.storageService = new Mock<IStorageService>();
        this.unitOfWork = new Mock<IUnitOfWork>();
        this.useCase = new UpdateVideo(this.videoRepository.Object, this.unitOfWork.Object, this.genreRepository.Object,
            this.categoryRepository.Object, this.castMemberRepository.Object, this.storageService.Object);
    }

    [Fact(DisplayName = nameof(UpdateVideosBasicInfo))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosBasicInfo()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var input = this.fixture.CreateValidInput(exampleVideo.Id);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepository.VerifyAll();
        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration),
            It.IsAny<CancellationToken>()), Times.Once);

        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }

    [Fact(DisplayName = nameof(UpdateThrowsWhenVideoNotFound))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateThrowsWhenVideoNotFound()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var input = this.fixture.CreateValidInput(exampleVideo.Id);
        this.videoRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Video not found"));

        var action = async () => await this.useCase.Handle(input, CancellationToken.None);
        await action.Should().ThrowAsync<NotFoundException>().WithMessage("Video not found");

        this.videoRepository.VerifyAll();
        this.videoRepository.Verify(repository => repository.Update(
            It.IsAny<Video>(),
            It.IsAny<CancellationToken>()), Times.Never);

        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory(DisplayName = nameof(UpdateVideosThrowsWhenReceiveInvalidInput))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    [ClassData(typeof(UpdateVideoTestDataGenerator))]
    public async Task UpdateVideosThrowsWhenReceiveInvalidInput(UpdateVideoInput invalidInput,
        string expectedExceptionMessage)
    {
        var exampleVideo = this.fixture.GetValidVideo();
        this.videoRepository.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var action = async () => await this.useCase.Handle(invalidInput, CancellationToken.None);

        var exceptionAssertion = await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage("There are validation errors");
        exceptionAssertion.Which.Errors!.ToList()[0].Message.Should().Be(expectedExceptionMessage);
        this.videoRepository.VerifyAll();


        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithGenreIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithGenreIds()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var exampleGenresIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, exampleGenresIds);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.genreRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleGenresIds.Count && idsList.All(id => exampleGenresIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenresIds);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepository.VerifyAll();
        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Genres.All(genreId => exampleGenresIds.Contains(genreId)) &&
                                  video.Genres.Count == exampleGenresIds.Count),
            It.IsAny<CancellationToken>()), Times.Once);

        this.genreRepository.VerifyAll();


        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(x => x.Id).Should().BeEquivalentTo(exampleGenresIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidGenreId))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenInvalidGenreId()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var exampleGenresIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var invalidGenreId = Guid.NewGuid();
        var inputInvalidIdsList = exampleGenresIds.Concat(new[] { invalidGenreId }).ToList();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, genresIds: inputInvalidIdsList);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.genreRepository.Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenresIds);

        var action = async () => await this.useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related Genre id (or ids) not found {invalidGenreId}.");
        this.videoRepository.VerifyAll();
        this.genreRepository.VerifyAll();

        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithCategoriesIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithCategoriesIds()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, categoryIds: exampleIds);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.categoryRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleIds.Count && idsList.All(id => exampleIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepository.VerifyAll();
        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Categories.All(categoryId => exampleIds.Contains(categoryId)) &&
                                  video.Categories.Count == exampleIds.Count),
            It.IsAny<CancellationToken>()), Times.Once);

        this.genreRepository.VerifyAll();


        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Categories.Select(x => x.Id).Should().BeEquivalentTo(exampleIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidCategoryId))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenInvalidCategoryId()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var invalidGenreId = Guid.NewGuid();
        var inputInvalidIdsList = exampleIds.Concat(new[] { invalidGenreId }).ToList();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, categoryIds: inputInvalidIdsList);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.categoryRepository.Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var action = async () => await this.useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related Category id (or ids) not found {invalidGenreId}.");
        this.videoRepository.VerifyAll();
        this.categoryRepository.VerifyAll();

        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithCastMemberIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithCastMemberIds()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, castMemberIds: exampleIds);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.castMemberRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleIds.Count && idsList.All(id => exampleIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepository.VerifyAll();
        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.CastMembers.All(castMemberId => exampleIds.Contains(castMemberId)) &&
                                  video.CastMembers.Count == exampleIds.Count),
            It.IsAny<CancellationToken>()), Times.Once);

        this.castMemberRepository.VerifyAll();


        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.CastMembers.Select(x => x.Id).Should().BeEquivalentTo(exampleIds);
    }


    [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidCastMemberIds))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosThrowsWhenInvalidCastMemberIds()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var exampleIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var invalidId = Guid.NewGuid();
        var inputInvalidIdsList = exampleIds.Concat(new[] { invalidId }).ToList();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, castMemberIds: inputInvalidIdsList);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.castMemberRepository.Setup(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleIds);

        var action = async () => await this.useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related CastMember id (or ids) not found {invalidId}.");
        this.videoRepository.VerifyAll();
        this.castMemberRepository.VerifyAll();

        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithoutRelationsWithRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithoutRelationsWithRelations()
    {
        var exampleVideo = this.fixture.GetValidVideo();
        var exampleGenresIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var exampleCastMembersIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, genresIds: exampleGenresIds,
            categoryIds: exampleCategoriesIds, castMemberIds: exampleCastMembersIds);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.genreRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleGenresIds.Count && idsList.All(id => exampleGenresIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenresIds);
        this.castMemberRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleCastMembersIds.Count &&
                        idsList.All(id => exampleCastMembersIds.Contains(id))), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMembersIds);
        this.categoryRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleCategoriesIds.Count &&
                        idsList.All(id => exampleCategoriesIds.Contains(id))), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepository.VerifyAll();
        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Genres.All(genreId => exampleGenresIds.Contains(genreId)) &&
                                  video.Genres.Count == exampleGenresIds.Count
                                  && video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId)) &&
                                  video.Categories.Count == exampleCategoriesIds.Count
                                  && video.CastMembers.All(castMemberId =>
                                      exampleCastMembersIds.Contains(castMemberId)) &&
                                  video.CastMembers.Count == exampleCastMembersIds.Count
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        this.genreRepository.VerifyAll();
        this.videoRepository.VerifyAll();
        this.castMemberRepository.VerifyAll();
        this.categoryRepository.VerifyAll();


        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(x => x.Id).Should().BeEquivalentTo(exampleGenresIds);
        output.Categories.Select(x => x.Id).Should().BeEquivalentTo(exampleCategoriesIds);
        output.CastMembers.Select(x => x.Id).Should().BeEquivalentTo(exampleCastMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsToOtherRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithRelationsToOtherRelations()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var exampleGenresIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var exampleCastMembersIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var exampleCategoriesIds = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, genresIds: exampleGenresIds,
            categoryIds: exampleCategoriesIds, castMemberIds: exampleCastMembersIds);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.genreRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleGenresIds.Count && idsList.All(id => exampleGenresIds.Contains(id))),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleGenresIds);
        this.castMemberRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleCastMembersIds.Count &&
                        idsList.All(id => exampleCastMembersIds.Contains(id))), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMembersIds);
        this.categoryRepository.Setup(x =>
                x.GetIdsByIds(
                    It.Is<List<Guid>>(idsList =>
                        idsList.Count == exampleCategoriesIds.Count &&
                        idsList.All(id => exampleCategoriesIds.Contains(id))), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCategoriesIds);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        this.videoRepository.VerifyAll();
        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Genres.All(genreId => exampleGenresIds.Contains(genreId)) &&
                                  video.Genres.Count == exampleGenresIds.Count
                                  && video.Categories.All(categoryId => exampleCategoriesIds.Contains(categoryId)) &&
                                  video.Categories.Count == exampleCategoriesIds.Count
                                  && video.CastMembers.All(castMemberId =>
                                      exampleCastMembersIds.Contains(castMemberId)) &&
                                  video.CastMembers.Count == exampleCastMembersIds.Count
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        this.genreRepository.VerifyAll();
        this.videoRepository.VerifyAll();
        this.castMemberRepository.VerifyAll();
        this.categoryRepository.VerifyAll();


        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating.ToStringSignal());
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(x => x.Id).Should().BeEquivalentTo(exampleGenresIds);
        output.Categories.Select(x => x.Id).Should().BeEquivalentTo(exampleCategoriesIds);
        output.CastMembers.Select(x => x.Id).Should().BeEquivalentTo(exampleCastMembersIds);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsRemovingRelations))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithRelationsRemovingRelations()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, genresIds: new(), categoryIds: new(),
            castMemberIds: new());
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Should().BeEmpty();
        output.Categories.Should().BeEmpty();
        output.CastMembers.Should().BeEmpty();

        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Genres.All(genreId => exampleVideo.Genres.Contains(genreId)) &&
                                  video.Genres.Count == exampleVideo.Genres.Count
                                  && video.Categories.All(categoryId => exampleVideo.Categories.Contains(categoryId)) &&
                                  video.Categories.Count == exampleVideo.Categories.Count
                                  && video.CastMembers.All(castMemberId =>
                                      exampleVideo.CastMembers.Contains(castMemberId)) &&
                                  video.CastMembers.Count == exampleVideo.CastMembers.Count
            ),
            It.IsAny<CancellationToken>()), Times.Once);
        this.videoRepository.VerifyAll();
        this.genreRepository.Verify(x => x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);
        this.castMemberRepository.Verify(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);
        this.categoryRepository.Verify(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithBannerWhenVideoHaveNoBanner))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithBannerWhenVideoHaveNoBanner()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, banner: this.fixture.GetImageValidFileInput());
        var bannerPath = $"storage/banner.{input.Banner.Extension}";
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.storageService.Setup(x => x.Upload(
            It.Is<string>(name => (
                name == StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Banner), input.Banner.Extension))),
            It.IsAny<Stream>(), It.IsAny<string>(),It.IsAny<CancellationToken>())).ReturnsAsync(bannerPath);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(bannerPath);

        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Banner.Path == bannerPath
            ),
            It.IsAny<CancellationToken>()), Times.Once);
        this.videoRepository.VerifyAll();
        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosKeepBannerWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepBannerWhenReceiveNull()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, banner: null);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.BannerFileUrl.Should().Be(exampleVideo.Banner.Path);

        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Banner.Path == exampleVideo.Banner.Path
            ),
            It.IsAny<CancellationToken>()), Times.Once);
        this.videoRepository.VerifyAll();
        this.storageService.Verify(
            x => x.Upload(It.IsAny<string>(), It.IsAny<FileStream>(), It.IsAny<string>(),It.IsAny<CancellationToken>()), Times.Never);
        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithRelationsKeepRelationsWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithRelationsKeepRelationsWhenReceiveNull()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, genresIds: null, categoryIds: null,
            castMemberIds: null);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.Genres.Select(x => x.Id).Should().BeEquivalentTo(exampleVideo.Genres);
        output.Categories.Select(x => x.Id).Should().BeEquivalentTo(exampleVideo.Categories);
        output.CastMembers.Select(x => x.Id).Should().BeEquivalentTo(exampleVideo.CastMembers);

        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
            ),
            It.IsAny<CancellationToken>()), Times.Once);
        this.videoRepository.VerifyAll();
        this.genreRepository.Verify(x => x.GetListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);
        this.castMemberRepository.Verify(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);
        this.categoryRepository.Verify(x => x.GetIdsByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithThumbWhenVideoHaveNoThumb))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithThumbWhenVideoHaveNoThumb()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, thumb: this.fixture.GetImageValidFileInput());
        var thumbPath = $"storage/thumb.{input.Thumb.Extension}";
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.storageService.Setup(x => x.Upload(
            It.Is<string>(name => (
                name == StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.Thumb), input.Thumb.Extension))),
            It.IsAny<Stream>(), It.IsAny<string>(),It.IsAny<CancellationToken>())).ReturnsAsync(thumbPath);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(thumbPath);

        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Thumb.Path == thumbPath
            ),
            It.IsAny<CancellationToken>()), Times.Once);
        this.videoRepository.VerifyAll();
        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosKeepThumbWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepThumbWhenReceiveNull()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, thumb: null);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(exampleVideo.Thumb.Path);

        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.Thumb.Path == exampleVideo.Thumb.Path
            ),
            It.IsAny<CancellationToken>()), Times.Once);
        this.videoRepository.VerifyAll();
        this.storageService.Verify(
            x => x.Upload(It.IsAny<string>(), It.IsAny<FileStream>(), It.IsAny<string>(),It.IsAny<CancellationToken>()), Times.Never);
        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosWithThumbHalfWhenVideoHaveNoThumbHalf))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosWithThumbHalfWhenVideoHaveNoThumbHalf()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, thumbHalf: this.fixture.GetImageValidFileInput());
        var thumbHalfPath = $"storage/thumbHalf.{input.ThumbHalf.Extension}";
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);
        this.storageService.Setup(x => x.Upload(
            It.Is<string>(name => (
                name == StorageFileName.Create(exampleVideo.Id, nameof(exampleVideo.ThumbHalf), input.ThumbHalf.Extension))),
            It.IsAny<Stream>(), It.IsAny<string>(),It.IsAny<CancellationToken>())).ReturnsAsync(thumbHalfPath);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbHalf.Should().Be(thumbHalfPath);

        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.ThumbHalf.Path == thumbHalfPath
            ),
            It.IsAny<CancellationToken>()), Times.Once);
        this.videoRepository.VerifyAll();
        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateVideosKeepThumbHalfWhenReceiveNull))]
    [Trait("Application", "UpdateVideo - Use Cases")]
    public async Task UpdateVideosKeepThumbHalfWhenReceiveNull()
    {
        var exampleVideo = this.fixture.GetValidVideoWithAllProperties();
        var input = this.fixture.CreateValidInput(exampleVideo.Id, thumbHalf: null);
        this.videoRepository.Setup(x => x.Get(It.Is<Guid>(id => id == exampleVideo.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleVideo);

        var output = await this.useCase.Handle(input, CancellationToken.None);

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default);
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
        output.ThumbFileUrl.Should().Be(exampleVideo.ThumbHalf.Path);

        this.videoRepository.Verify(repository => repository.Update(
            It.Is<Video>(video => video.Id == input.VideoId
                                  && video.Title == input.Title
                                  && video.Description == input.Description
                                  && video.Rating == input.Rating
                                  && video.YearLaunched == input.YearLaunched
                                  && video.Opened == input.Opened
                                  && video.Published == input.Published
                                  && video.Duration == input.Duration
                                  && video.ThumbHalf.Path == exampleVideo.ThumbHalf.Path
            ),
            It.IsAny<CancellationToken>()), Times.Once);
        this.videoRepository.VerifyAll();
        this.storageService.Verify(
            x => x.Upload(It.IsAny<string>(), It.IsAny<FileStream>(), It.IsAny<string>(),It.IsAny<CancellationToken>()), Times.Never);
        this.unitOfWork.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }


}