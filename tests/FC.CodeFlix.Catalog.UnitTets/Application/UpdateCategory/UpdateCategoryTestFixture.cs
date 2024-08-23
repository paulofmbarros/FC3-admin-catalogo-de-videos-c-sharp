// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.UpdateCategory;

using Domain.Common;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Moq;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture>
{
}

public class UpdateCategoryTestFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
        {
            categoryName = Faker.Commerce.Categories(1)[0];
        }

        if (categoryName.Length > 255)
        {
            categoryName = categoryName.Substring(0, 255);
        }

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10_000)
        {
            categoryDescription = categoryDescription.Substring(0, 10000);
        }

        return categoryDescription;
    }

    public bool GetRandomBoolean() => Faker.Random.Bool();

    public Category GetValidCategory()
    {
        var category = new Category(this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());
        return category;
    }

    public UpdateCategoryInput GenerateUpdateCategoryInput(Guid? id = null)
    {
        return new UpdateCategoryInput(id ?? Guid.NewGuid(), this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());
    }

    public UpdateCategoryInput GetInvalidInputLongDescription()
    {
        // description não pode ser maior que 10_000 caracteres
        var invalidInputLongDescription = this.GenerateUpdateCategoryInput();
        invalidInputLongDescription = invalidInputLongDescription with { Description = new string('a', 10_001) };
        return invalidInputLongDescription;
    }

    public UpdateCategoryInput GetInvalidInputLongName()
    {
        // nome nao pode ser maior que 255 caracteres
        var invalidInputLongName = this.GenerateUpdateCategoryInput();
        invalidInputLongName = invalidInputLongName with { Name = new string('a', 256) };
        return invalidInputLongName;
    }

    public UpdateCategoryInput GetInvalidInputShortName()
    {
        // nome nao pode ser menor 3 caracteres
        var invalidInputShortName = this.GenerateUpdateCategoryInput();
        invalidInputShortName = invalidInputShortName with { Name = invalidInputShortName.Name.Substring(0, 2) };
        return invalidInputShortName;
    }
    public  UpdateCategoryInput GetInvalidInputNullName()
    {
        //nome não pode ser null
        var invalidInputNullName = this.GenerateUpdateCategoryInput();
        invalidInputNullName = invalidInputNullName with { Name = null };
        return invalidInputNullName;
    }

    public Mock<ICategoryRepository> GetRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}