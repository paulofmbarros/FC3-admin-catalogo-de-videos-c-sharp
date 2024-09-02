// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.CreateCategory;

using Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using Fc.CodeFlix.Catalog.Domain.Entity;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture>
{
}


public class CreateCategoryTestFixture : CategoryUseCasesBaseFixture
{
    public CreateCategoryInput GetInput() => new (this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());

    public CreateCategoryInput GetInvalidInputLongDescription()
    {
        // description não pode ser maior que 10_000 caracteres
        var invalidInputLongDescription = this.GetInput();
        invalidInputLongDescription = invalidInputLongDescription with { Description = new string('a', 10_001) };
        return invalidInputLongDescription;
    }

    public CreateCategoryInput GetInvalidInputNullDescription()
    {
        var invalidInputNullDescription =this.GetInput();
        invalidInputNullDescription = invalidInputNullDescription with { Description = null };
        return invalidInputNullDescription;
    }

    public CreateCategoryInput GetInvalidInputLongName()
    {
        // nome nao pode ser maior que 255 caracteres
        var invalidInputLongName = this.GetInput();
        invalidInputLongName = invalidInputLongName with { Name = new string('a', 256) };
        return invalidInputLongName;
    }

    public CreateCategoryInput GetInvalidInputShortName()
    {
        // nome nao pode ser menor 3 caracteres
        var invalidInputShortName = this.GetInput();
        invalidInputShortName = invalidInputShortName with { Name = invalidInputShortName.Name.Substring(0, 2) };
        return invalidInputShortName;
    }
    public  CreateCategoryInput GetInvalidInputNullName()
    {
        //nome não pode ser null
        var invalidInputNullName = this.GetInput();
        invalidInputNullName = invalidInputNullName with { Name = null };
        return invalidInputNullName;
    }

    public Category GetValidCategory()
    {
        var category = new Category(this.GetValidCategoryName(), this.GetValidCategoryDescription());
        return category;
    }



}