// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;

using Domain.Repository;
using Interfaces;
using MediatR;

public class DeleteCategory : IDeleteCategory
{
    private readonly ICategoryRepository categoryRepository;
    private readonly IUnitOfWork unitOfWork;


    public DeleteCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        this.categoryRepository = categoryRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.Get(request.Id, cancellationToken);
        await this.categoryRepository.Delete(category, cancellationToken);
        await this.unitOfWork.Commit(cancellationToken);

        return Unit.Value;
    }
}