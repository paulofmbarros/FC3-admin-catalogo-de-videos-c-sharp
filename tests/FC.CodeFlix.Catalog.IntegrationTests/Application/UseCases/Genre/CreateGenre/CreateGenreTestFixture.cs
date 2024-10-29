// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

using Common;
using Fc.CodeFlix.Catalog.Application.UseCases.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureColecion : ICollectionFixture<CreateGenreTestFixture>
{

}


public class CreateGenreTestFixture : GenreUseCasesBaseFixture
{
    public CreateGenreInput GetExampleInput() => new(this.GetGenreName(), this.GetRandomBoolean());
}