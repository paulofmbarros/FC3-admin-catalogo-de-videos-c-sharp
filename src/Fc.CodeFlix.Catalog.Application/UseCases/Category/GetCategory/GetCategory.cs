// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;

using Common;
using Domain.Repository;
using MediatR;

public class GetCategory : IGetCategory
{
    private readonly ICategoryRepository categoryRepository;

    public GetCategory(ICategoryRepository categoryRepository) => this.categoryRepository = categoryRepository;


    public async Task<CategoryModelOutput> Handle(GetCategoryInput request, CancellationToken cancellationToken)
    {
        var category = await this.categoryRepository.Get(request.Id, cancellationToken);

        return CategoryModelOutput.FromCategory(category);
    }
}