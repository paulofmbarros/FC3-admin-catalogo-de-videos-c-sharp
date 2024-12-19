// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

using Infra.Data.EF;

public class GenrePersistence(CodeflixCatalogDbContext dbContext)
{
    private readonly CodeflixCatalogDbContext dbContext = dbContext;
}