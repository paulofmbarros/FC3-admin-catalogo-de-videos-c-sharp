// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

using Application.Common;
using Common;
using Domain.Entity;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Validation;
using Exceptions;
using Interfaces;

public class CreateVideo : ICreateVideo
{
    private readonly IVideoRepository videoRepository;

    private readonly ICategoryRepository categoryRepository;

    private readonly IGenreRepository genreRepository;

    private readonly ICastMemberRepository castMemberRepository;

    private readonly IStorageService storageService;

    private readonly IUnitOfWork unitOfWork;

    public CreateVideo(IVideoRepository videoRepository, ICategoryRepository categoryRepository,
        IGenreRepository genreRepository,ICastMemberRepository castMemberRepository, IUnitOfWork unitOfWork, IStorageService storageService)
    {
        this.videoRepository = videoRepository;
        this.unitOfWork = unitOfWork;
        this.storageService = storageService;
        this.castMemberRepository = castMemberRepository;
        this.categoryRepository = categoryRepository;
        this.genreRepository = genreRepository;
    }

    public async Task<VideoModelOutput> Handle(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var video = new Video(input.Title,
            input.Description,
            input.Opened,
            input.Published,
            input.YearLaunched,
            input.Duration,
            input.Rating);

        var validationHandler = new NotificationValidationHandler();
        video.Validate(validationHandler);

        if (validationHandler.HasErrors())
        {
            throw new EntityValidationException("There are validation errors", validationHandler.Errors);
        }

        await this.ValidateAndAddRelations(input, cancellationToken, video);

        try
        {
            await this.UploadImagesMedia(input, cancellationToken, video);
            await this.UploadVideosMedia(input, cancellationToken, video);


            await this.videoRepository.Insert(video, cancellationToken);
            await this.unitOfWork.Commit(cancellationToken);

            return VideoModelOutput.FromVideo(video);
        }
        catch (Exception e)
        {
            await this.ClearStorage(cancellationToken, video);
            throw;
        }


    }
    private async Task ClearStorage(CancellationToken cancellationToken, Video video)
    {
        if(video.Thumb is not null)
            await this.storageService.Delete(video.Thumb.Path, cancellationToken);
        if(video.ThumbHalf is not null)
            await this.storageService.Delete(video.ThumbHalf.Path, cancellationToken);
        if(video.Banner is not null)
            await this.storageService.Delete(video.Banner.Path, cancellationToken);
        if(video.Media is not null)
            await this.storageService.Delete(video.Media.FilePath, cancellationToken);
        if(video.Trailer is not null)
            await this.storageService.Delete(video.Trailer.FilePath, cancellationToken);
    }

    private async Task UploadImagesMedia(CreateVideoInput input, CancellationToken cancellationToken, Video video)
    {
        if (input.Thumb is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Thumb), input.Thumb.Extension);

            var thumbUrl = await this.storageService.Upload(
                fileName,
                input.Thumb.FileStream,
                input.Thumb.ContentType,
                cancellationToken);

            video.UpdateThumb(thumbUrl);
        }
        if (input.Banner is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Banner), input.Banner.Extension);
            var bannerUrl = await this.storageService.Upload(
                fileName,
                input.Banner.FileStream,
                input.Banner.ContentType,
                cancellationToken);

            video.UpdateBanner(bannerUrl);
        }

        if (input.ThumbHalf is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.ThumbHalf), input.ThumbHalf.Extension);
            var thumbHalfUrl = await this.storageService.Upload(
                fileName,
                input.ThumbHalf.FileStream,
                input.ThumbHalf.ContentType,
                cancellationToken);

            video.UpdateThumbHalf(thumbHalfUrl);
        }
    }

    private async Task UploadVideosMedia(CreateVideoInput input, CancellationToken cancellationToken, Video video)
    {
        if (input.Media is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Media), input.Media.Extension);
            var mediaUrl = await this.storageService.Upload(
                fileName,
                input.Media.FileStream,
                input.Media.ContentType,
                cancellationToken);

            video.UpdateMedia(mediaUrl);
        }

        if (input.Trailer is not null)
        {
            var fileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.Trailer.Extension);
            var trailerUrl = await this.storageService.Upload(
                fileName,
                input.Trailer.FileStream,
                input.Trailer.ContentType,
                cancellationToken);

            video.UpdateTrailer(trailerUrl);
        }
    }


    private async Task ValidateAndAddRelations(CreateVideoInput input, CancellationToken cancellationToken, Video video)
    {
        if (input.CategoriesIds is not null && input.CategoriesIds.Count > 0)
        {
            await this.ValidateCategoryIds(input, cancellationToken);

            foreach (var categoryId in input.CategoriesIds)
            {
                video.AddCategory(categoryId);
            }
        }

        if (input.GenreIds is not null && input.GenreIds.Count > 0)
        {
            await this.ValidateGenreIds(input, cancellationToken);

            foreach (var genreId in input.GenreIds)
            {
                video.AddGenre(genreId);
            }
        }

        if (input.CastMembersIds is not null && input.CastMembersIds.Count > 0)
        {
            await this.ValidateCastMemberIds(input, cancellationToken);

            foreach (var castMembersId in input.CastMembersIds)
            {
                video.AddCastMember(castMembersId);
            }
        }
    }

    private async Task ValidateCastMemberIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds =
            await this.castMemberRepository.GetIdsByIds(input.CastMembersIds.ToList(), cancellationToken);

        if (persistenceIds.Count < input.CastMembersIds.Count)
        {
            var notFoundCastMembersIds = input.CastMembersIds.Except(persistenceIds).ToList();
            throw new RelatedAggregateException(
                $"Related CastMember id (or ids) not found {string.Join(',', notFoundCastMembersIds)}.");
        }
    }

    private async Task ValidateGenreIds(CreateVideoInput input, CancellationToken cancellationToken)
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

    private async Task ValidateCategoryIds(CreateVideoInput input, CancellationToken cancellationToken)
    {
        var persistenceIds =
            await this.categoryRepository.GetIdsByIds(input.CategoriesIds.ToList(), cancellationToken);

        if (persistenceIds.Count < input.CategoriesIds.Count)
        {
            var notFoundCategoriesIds = input.CategoriesIds.Except(persistenceIds).ToList();
            throw new RelatedAggregateException(
                $"Related Category id (or ids) not found {string.Join(',', notFoundCategoriesIds)}.");
        }
    }
}