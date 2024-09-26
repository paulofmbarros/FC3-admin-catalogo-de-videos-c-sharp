// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Category.GetCategory;

using Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using Xunit;

[CollectionDefinition(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiTestFixtureCollection : ICollectionFixture<GetCategoryApiTestFixture>
{

}


public class GetCategoryApiTestFixture : CategoryBaseFixture
{
}