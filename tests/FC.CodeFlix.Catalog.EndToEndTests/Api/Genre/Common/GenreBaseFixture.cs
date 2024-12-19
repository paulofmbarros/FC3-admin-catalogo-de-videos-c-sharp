// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

using Base;

public class GenreBaseFixture : BaseFixture
{
    public GenrePersistence Persistence { get; set; }

    public GenreBaseFixture() : base()
    {
        this.Persistence = new GenrePersistence(this.CreateDbContext());
    }
}