// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

public class VideosCastMembersConfiguration : IEntityTypeConfiguration<VideosCastMembers>
{
    public void Configure(EntityTypeBuilder<VideosCastMembers> builder) =>
        builder.HasKey(relation => new
    {
        relation.VideoId,
        relation.CastMemberId
    });
}