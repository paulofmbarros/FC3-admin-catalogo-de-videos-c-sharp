// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Validator;

using Entity;
using Validation;

public class VideoValidator : Validator
{
    private readonly Video video;

    private const int TitleMaxLength = 255;
    private const int DescriptionMaxLength = 4_000;
    public VideoValidator(Video video, ValidationHandler handler) : base(handler) => this.video = video;

    public override void Validate()
    {
        this.ValidateTitle();
        this.ValidateDescription();
    }

    private void ValidateDescription()
    {
        if (string.IsNullOrWhiteSpace(this.video.Description))
        {
            this.Handler.HandleError($"{nameof(this.video.Description)} should not be empty.");
        }

        if (this.video.Description.Length > DescriptionMaxLength)
        {
            this.Handler.HandleError($"{nameof(this.video.Description)} should be less or equal {DescriptionMaxLength} characters long.");
        }
    }

    private void ValidateTitle()
    {
        if (this.video.Title.Length > TitleMaxLength)
        {
            this.Handler.HandleError($"{nameof(this.video.Title)} should be less or equal {TitleMaxLength} characters long.");
        }

        if (string.IsNullOrWhiteSpace(this.video.Title))
        {
            this.Handler.HandleError($"{nameof(this.video.Title)} should not be null or empty.");
        }
    }
}