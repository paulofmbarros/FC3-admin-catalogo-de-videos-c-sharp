// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;

using MediatR;

public interface IDeleteCategory : IRequestHandler<DeleteCategoryInput, Unit>
{
    
}