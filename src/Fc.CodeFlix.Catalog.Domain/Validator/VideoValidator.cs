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
    public VideoValidator(Video video, ValidationHandler handler) : base(handler) => this.video = video;

    public override void Validate()
    {
        if (this.video.Title.Length > 255)
        {
            this.Handler.HandleError($"{nameof(this.video.Title)} should be less or equal {TitleMaxLength} characters long.");
        }
    }
}