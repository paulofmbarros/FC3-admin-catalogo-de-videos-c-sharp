// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF;

using Configurations;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Models;

public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GenresCategories> GenresCategories => Set<GenresCategories>();
    public DbSet<VideosCategories> VideosCategories => Set<VideosCategories>();
    public DbSet<VideosGenres> VideosGenres=> Set<VideosGenres>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<VideosCastMembers> VideosCastMembers => Set<VideosCastMembers>();

    public DbSet<Video> Videos => Set<Video>();

    public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelos
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new GenreConfiguration());
        modelBuilder.ApplyConfiguration(new VideoConfiguration());
        modelBuilder.ApplyConfiguration(new CastMemberConfiguration());

        //relacionamentos
        modelBuilder.ApplyConfiguration(new GenresCategoriesConfiguration());
        modelBuilder.ApplyConfiguration(new VideosCategoriesConfiguration());
        modelBuilder.ApplyConfiguration(new VideosGenresConfiguration());
        modelBuilder.ApplyConfiguration(new VideosCastMembersConfiguration());
    }

}
