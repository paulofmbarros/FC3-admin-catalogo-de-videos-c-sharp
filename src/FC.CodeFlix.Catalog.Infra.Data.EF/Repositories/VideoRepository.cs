// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;
using Models;

public class VideoRepository : IVideoRepository
{
    private readonly CodeflixCatalogDbContext context;
    private DbSet<Video> Videos => context.Videos;
    private DbSet<VideosCategories> VideoCategories => this.context.VideosCategories;
    private DbSet<VideosGenres> VideosGenres => this.context.VideosGenres;
    private DbSet<VideosCastMembers> VideosCastMembers => this.context.VideosCastMembers;

    public VideoRepository(CodeflixCatalogDbContext context) => this.context = context;

    public async Task Insert(Video video, CancellationToken cancellationToken)
    {
        await this.Videos.AddAsync(video, cancellationToken);
        if(video.Categories.Count > 0)
        {
            var relations = video.Categories.Select(categoryId => new VideosCategories(categoryId, video.Id));
            await this.VideoCategories.AddRangeAsync(relations, cancellationToken);
        }
        if(video.Genres.Count > 0)
        {
            var relations = video.Genres.Select(genreId => new VideosGenres(genreId, video.Id));
            await this.VideosGenres.AddRangeAsync(relations, cancellationToken);
        }
        if(video.CastMembers.Count > 0)
        {
            var relations = video.CastMembers.Select(castMemberId => new VideosCastMembers(castMemberId, video.Id));
            await this.VideosCastMembers.AddRangeAsync(relations, cancellationToken);
        }
    }

    public Task<Video> Get(Guid id, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task Delete(Video aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task Update(Video aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<SearchOutput<Video>> Search(SearchInput searchInput, CancellationToken cancellationToken) => throw new NotImplementedException();
}