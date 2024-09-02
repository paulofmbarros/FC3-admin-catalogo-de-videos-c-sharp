// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.ListCategories;

using MediatR;

public interface IListCategories : IRequestHandler<ListCategoriesInput,ListCategoriesOutput>
{
    
}