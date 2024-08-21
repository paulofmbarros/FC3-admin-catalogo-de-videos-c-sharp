// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;

using Common;
using MediatR;

public class GetCategoryInput : IRequest<CategoryModelOutput>
{
    public Guid Id { get; set; }

    public GetCategoryInput(Guid id)
    {
        this.Id = id;
    }
}