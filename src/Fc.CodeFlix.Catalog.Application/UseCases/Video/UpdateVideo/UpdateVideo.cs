// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;

using Application.Common;
using Common;
using Domain.Entity;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Validation;
using Exceptions;
using Interfaces;

public class UpdateVideo : IUpdateVideo
{
    private readonly IVideoRepository videoRepository;
    private readonly IGenreRepository genreRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly ICastMemberRepository castMemberRepository;
    private readonly IStorageService storageService;
    private readonly IUnitOfWork unitOfWork;

    public UpdateVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork, IGenreRepository genreRepository, ICategoryRepository categoryRepository, ICastMemberRepository castMemberRepository, IStorageService storageService)
    {
        this.videoRepository = videoRepository;
        this.unitOfWork = unitOfWork;
        this.genreRepository = genreRepository;
        this.categoryRepository = categoryRepository;
        this.castMemberRepository = castMemberRepository;
        this.storageService = storageService;
    }
    public async Task<VideoModelOutput> Handle(UpdateVideoInput input, CancellationToken cancellationToken)
    {

        var video =  await this.videoRepository.Get(input.VideoId, cancellationToken);
        var validationHandler = new NotificationValidationHandler();
        video.Update(input.Title, input.Description, input.Opened, input.Published, input.YearLaunched, input.Duration, input.Rating);


        video.Validate(validationHandler);

        if (validationHandler.HasErrors())
        {
            throw new EntityValidationException("There are validation errors", validationHandler.Errors);
        }

        await this.ValidateAndAddRelations(input, cancellationToken, video);

        await this.UploadImagesMedia(video, input, cancellationToken);

        await this.videoRepository.Update(video, cancellationToken);
        await this.unitOfWork.Commit(cancellationToken);

        return VideoModelOutput.FromVideo(video);
    }

    private async Task ValidateAndAddRelations(UpdateVideoInput input, CancellationToken cancellationToken, Video video)
    {
        if (input.GenreIds is not null)
        {
            video.RemoveAllGenres();
            if (input.GenreIds.Any())
            {
                await this.ValidateGenreIds(input, cancellationToken);
                input.GenreIds.ToList().ForEach(video.AddGenre);
            }
        }

        if (input.CategoryIds is not null)
        {
            video.RemoveAllCategories();
            if (input.CategoryIds.Any())
            {
                await this.ValidateCategoryIds(input, cancellationToken);
                input.CategoryIds.ToList().ForEach(video.AddCategory);
            }
        }

        if (input.CastMemberIds is not null)
        {
            video.RemoveAllCastMembers();
            if (input.CastMemberIds.Any())
            {
                await this.ValidateCastMemberIds(input, cancellationToken);
                foreach (var castMembersId in input.CastMemberIds)
                {
                    video.AddCastMember(castMembersId);
                }
            }


        }
    }

    private async Task ValidateGenreIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds =
            await this.genreRepository.GetIdsByIds(input.GenreIds.ToList(), cancellationToken);

        if (persistenceIds.Count < input.GenreIds.Count)
        {
            var notFoundGenresIds = input.GenreIds.Except(persistenceIds).ToList();
            throw new RelatedAggregateException(
                $"Related Genre id (or ids) not found {string.Join(',', notFoundGenresIds)}.");
        }
    }

    private async Task ValidateCategoryIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds =
            await this.categoryRepository.GetIdsByIds(input.CategoryIds.ToList(), cancellationToken);

        if (persistenceIds.Count < input.CategoryIds.Count)
        {
            var notFoundCategoriesIds = input.CategoryIds.Except(persistenceIds).ToList();
            throw new RelatedAggregateException(
                $"Related Category id (or ids) not found {string.Join(',', notFoundCategoriesIds)}.");
        }
    }

    private async Task ValidateCastMemberIds(UpdateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds =
            await this.castMemberRepository.GetIdsByIds(input.CastMemberIds.ToList(), cancellationToken);

        if (persistenceIds.Count < input.CastMemberIds.Count)
        {
            var notFoundCastMembersIds = input.CastMemberIds.Except(persistenceIds).ToList();
            throw new RelatedAggregateException(
                $"Related CastMember id (or ids) not found {string.Join(',', notFoundCastMembersIds)}.");
        }
    }

    private async Task UploadImagesMedia(Video video, UpdateVideoInput input, CancellationToken cancellationToken)
    {
        if (input.Banner is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.Banner.Extension);
            var mediaUrl = await this.storageService.Upload(
                fileName,
                input.Banner.FileStream,
                cancellationToken);

            video.UpdateBanner(mediaUrl);
        }

        if (input.Thumb is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.Thumb.Extension);
            var thumbUrl = await this.storageService.Upload(
                fileName,
                input.Thumb.FileStream,
                cancellationToken);

            video.UpdateThumb(thumbUrl);
        }

        if (input.ThumbHalf is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalf.Extension);
            var thumbHalfUrl = await this.storageService.Upload(
                fileName,
                input.ThumbHalf.FileStream,
                cancellationToken);

            video.UpdateThumbHalf(thumbHalfUrl);
        }
    }
}