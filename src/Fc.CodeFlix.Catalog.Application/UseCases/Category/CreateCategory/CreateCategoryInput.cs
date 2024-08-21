// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

using Common;
using MediatR;

public record CreateCategoryInput(string Name, string? Description, bool? IsActive) : IRequest<CategoryModelOutput>;