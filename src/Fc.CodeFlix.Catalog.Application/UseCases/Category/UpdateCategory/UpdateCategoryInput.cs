// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;

using Common;
using MediatR;

public record UpdateCategoryInput(
    Guid Id,
    string Name,
    string? Description = null,
    bool? IsActive = null) : IRequest<CategoryModelOutput>;