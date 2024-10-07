// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

using Common;
using Fc.CodeFlix.Catalog.Api.ApiModels.Category;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using Xunit;

[CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture>
{

}


public class UpdateCategoryApiTestFixture : CategoryBaseFixture
{
    public UpdateCategoryApiInput GetExampleInput() =>
        new(
            this.GetValidCategoryName(),
            this.GetValidCategoryDescription(),
            this.GetRandomBoolean()
        );
}