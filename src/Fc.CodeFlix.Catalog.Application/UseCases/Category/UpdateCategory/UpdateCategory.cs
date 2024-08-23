// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;

using Common;
using Domain.Repository;
using Interfaces;

public class UpdateCategory : IUpdateCategory
{
    private readonly ICategoryRepository categoryRepository;
    private readonly IUnitOfWork unitOfWork;

    public UpdateCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        this.categoryRepository = categoryRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<CategoryModelOutput> Handle(UpdateCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await this.categoryRepository.Get(request.Id, cancellationToken);
        category.Update(request.Name, request.Description);

        if(request.IsActive != null && request.IsActive != category.IsActive)
        {
            if ( request.IsActive.Value)
            {
                category.Activate();
            }
            else
            {
                category.Deactivate();
            }
        }

        await this.categoryRepository.Update(category, cancellationToken);
        await this.unitOfWork.Commit(cancellationToken);

        return CategoryModelOutput.FromCategory(category);

    }
}