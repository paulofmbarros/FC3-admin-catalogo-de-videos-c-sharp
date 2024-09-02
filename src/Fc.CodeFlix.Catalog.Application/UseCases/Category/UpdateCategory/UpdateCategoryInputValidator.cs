// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;

using FluentValidation;

public class UpdateCategoryInputValidator : AbstractValidator<UpdateCategoryInput>
{
    public UpdateCategoryInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name must be less than 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(10_000).WithMessage("Description must be less than 10000 characters");
    }
}