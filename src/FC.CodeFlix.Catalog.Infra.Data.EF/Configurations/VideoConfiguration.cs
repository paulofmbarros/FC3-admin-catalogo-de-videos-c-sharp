// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Configurations;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(video => video.Id);
        builder.Property(video => video.Title).HasMaxLength(255);
        builder.Property(video => video.Description).HasMaxLength(4000);
        builder.OwnsOne(video => video.Thumb, thumb =>
        {
            thumb.Property(image => image.Path).HasColumnName("ThumbPath").IsRequired(false);
        });
        builder.OwnsOne(video => video.ThumbHalf, thumbHalf =>
        {
            thumbHalf.Property(image => image.Path).HasColumnName("ThumbHalfPath").IsRequired(false);
        });

        builder.OwnsOne(video => video.Banner, banner =>
        {
            banner.Property(image => image.Path).HasColumnName("bannerPath").IsRequired(false);
        });
    }
}