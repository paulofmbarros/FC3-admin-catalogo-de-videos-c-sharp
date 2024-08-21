// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;

using FluentValidation;

public class GetCategoryInputValidator : AbstractValidator<GetCategoryInput>
{
    public GetCategoryInputValidator()
    {
        this.RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}