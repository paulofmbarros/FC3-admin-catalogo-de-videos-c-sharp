// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.Category.UpdateCategory;

using Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture>
{
}

public class UpdateCategoryTestFixture : CategoryUseCasesBaseFixture
{
    public UpdateCategoryInput GenerateUpdateCategoryInput(Guid? id = null)
    {
        return new UpdateCategoryInput(id ?? Guid.NewGuid(), this.GetValidCategoryName(),
            this.GetValidCategoryDescription(), this.GetRandomBoolean());
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

}