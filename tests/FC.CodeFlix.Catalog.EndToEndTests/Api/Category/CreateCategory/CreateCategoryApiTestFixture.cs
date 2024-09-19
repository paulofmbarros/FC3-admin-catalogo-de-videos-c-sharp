// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.CreateCategory;

using Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTestFixtureCollection : ICollectionFixture<CreateCategoryApiTestFixture>
{

}


public class CreateCategoryApiTestFixture : CategoryBaseFixture
{
    public CreateCategoryInput GetExampleInput()
        => new (this.GetValidCategoryName(), this.GetValidCategoryDescription(), this.GetRandomBoolean());

}