// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

using Domain.Entity;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Validation;
using Interfaces;

public class CreateVideo : ICreateVideo
{
    private readonly IVideoRepository videoRepository;

    private readonly IUnitOfWork unitOfWork;

    public CreateVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork)
    {
        this.videoRepository = videoRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<CreateVideoOutput> Handle(CreateVideoInput input, CancellationToken cancellationToken)
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

        await this.videoRepository.Insert(video, cancellationToken);
        await this.unitOfWork.Commit(cancellationToken);

        return new CreateVideoOutput(
            video.Id,
            video.CreatedAt,
            video.Title,
            video.Published,
            video.Description,
            video.Rating,
            video.YearLaunched,
            video.Opened,
            video.Duration);

    }
}