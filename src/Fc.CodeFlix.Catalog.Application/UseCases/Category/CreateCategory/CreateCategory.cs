﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

using Common;
using Domain.Entity;
using Domain.Repository;
using Interfaces;

public class CreateCategory : ICreateCategory
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ICategoryRepository categoryRepository;

    public CreateCategory(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
    {
        this.unitOfWork = unitOfWork;
        this.categoryRepository = categoryRepository;
    }

    public async Task<CategoryModelOutput> Handle(CreateCategoryInput input, CancellationToken cancellationToken)
    {
        var category = new Category(input.Name, input.Description, input.IsActive);

        await this.categoryRepository.Insert(category, cancellationToken);

        await this.unitOfWork.Commit(cancellationToken);

        return CategoryModelOutput.FromCategory(category);
    }
}