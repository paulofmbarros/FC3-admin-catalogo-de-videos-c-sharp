// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

public interface ICreateCategory
{
    Task<CreateCategoryOutput> Handle(CreateCategoryInput input, CancellationToken cancellationToken);
}