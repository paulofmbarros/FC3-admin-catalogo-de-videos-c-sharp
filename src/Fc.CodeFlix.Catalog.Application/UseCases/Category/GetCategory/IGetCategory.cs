// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;

using Common;
using MediatR;

public interface IGetCategory : IRequestHandler<GetCategoryInput,CategoryModelOutput>
{
    
}