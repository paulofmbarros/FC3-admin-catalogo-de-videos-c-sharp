// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;

using MediatR;

public class DeleteCategoryInput : IRequest<Unit>
{
    public DeleteCategoryInput(Guid id)
    {
        this.Id = id;
    }

    public Guid Id { get; set; }
}