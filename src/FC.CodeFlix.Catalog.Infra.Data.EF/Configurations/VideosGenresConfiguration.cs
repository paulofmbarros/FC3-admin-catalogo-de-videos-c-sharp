// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

public class VideosGenresConfiguration: IEntityTypeConfiguration<VideosGenres>
{
        public void Configure(EntityTypeBuilder<VideosGenres> builder)
            =>    builder.HasKey(relation => new { relation.VideoId, relation.GenreId });

}