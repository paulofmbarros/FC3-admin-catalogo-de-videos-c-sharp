// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Repositories;

using Fc.CodeFlix.Catalog.Application.Exceptions;
using Fc.CodeFlix.Catalog.Domain.Entity;
using Fc.CodeFlix.Catalog.Domain.Repository;
using Fc.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;
using Models;

public class VideoRepository : IVideoRepository
{
    private readonly CodeflixCatalogDbContext context;
    private DbSet<Video> Videos => context.Videos;
    private DbSet<Media> Medias => context.Set<Media>();
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

    public async Task<Video> Get(Guid id, CancellationToken cancellationToken)
    {
        var video = await this.Videos.FirstOrDefaultAsync(v => v.Id == id);
        NotFoundException.ThrowIfNull(video, $"Video '{id}' not found.");
        var categoriesIds = await this.VideoCategories
            .Where(x=>x.VideoId == id)
            .Select(x=>x.CategoryId)
            .ToListAsync(cancellationToken);
        categoriesIds.ForEach(video.AddCategory);

        var genresIds = await this.VideosGenres
            .Where(x=>x.VideoId == id)
            .Select(x=>x.GenreId)
            .ToListAsync(cancellationToken);
        genresIds.ForEach(video.AddGenre);

        var castMembersIds = await this.VideosCastMembers
            .Where(x=>x.VideoId == id)
            .Select(x=>x.CastMemberId)
            .ToListAsync(cancellationToken);
        castMembersIds.ForEach(video.AddCastMember);
        return video;

    }

    public  Task Delete(Video video, CancellationToken cancellationToken)
    {
        this.VideoCategories.RemoveRange(this.VideoCategories.Where(x=> x.VideoId == video.Id));
        this.VideosCastMembers.RemoveRange(this.VideosCastMembers.Where(x=> x.VideoId == video.Id));
        this.VideosGenres.RemoveRange(this.VideosGenres.Where(x=> x.VideoId == video.Id));

        if (video.Trailer is not null)
        {
            this.Medias.Remove(video.Trailer);
        }

        if (video.Media is not null)
        {
            this.Medias.Remove(video.Media);
        }

        this.Videos.Remove(video);
        return Task.CompletedTask;
    }

    public async Task Update(Video video, CancellationToken cancellationToken)
    {
        this.Videos.Update(video);

        this.VideoCategories.RemoveRange(this.VideoCategories.Where(x=> x.VideoId == video.Id));
        this.VideosCastMembers.RemoveRange(this.VideosCastMembers.Where(x=> x.VideoId == video.Id));
        this.VideosGenres.RemoveRange(this.VideosGenres.Where(x=> x.VideoId == video.Id));
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

    public async Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = input.PerPage * (input.Page - 1);
        var query = this.Videos.AsNoTracking();

        query = this.AddOrderToQuery(query, input.OrderBy, input.Order);

        if (string.IsNullOrWhiteSpace(input.Search) is false)
        {
            query = query
                .Where(category => category.Title.Contains(input.Search));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        var videoIds = items.Select(x => x.Id).ToList();

        await this.AddCategoriesToVideos(items, videoIds, cancellationToken);
        await this.AddGenresToVideos(items, videoIds, cancellationToken);
        await this.AddCastMembersToVideos(items, videoIds, cancellationToken);

        return new SearchOutput<Video>(input.Page, input.PerPage, total, items);
    }

    private async Task AddCategoriesToVideos(List<Video> items, List<Guid> videoIds, CancellationToken cancellationToken)
    {
        var categoriesRelations  = await this.VideoCategories
            .Where(relation => videoIds
                .Contains(relation.VideoId))
            .ToListAsync(cancellationToken);

        var relationWithCategoriesByVideoId = categoriesRelations.GroupBy(x => x.VideoId).ToList();

        foreach (var relationGroup in relationWithCategoriesByVideoId)
        {
            var video = items.Find(x => x.Id == relationGroup.Key);

            foreach (var relation in relationGroup)
            {
                video.AddCategory(relation.CategoryId);
            }
        }
    }

    private async Task AddGenresToVideos(List<Video> items, List<Guid> videoIds, CancellationToken cancellationToken)
    {
        var genresRelations  = await this.VideosGenres
            .Where(relation => videoIds
                .Contains(relation.VideoId))
            .ToListAsync(cancellationToken);

        var relationsWithGenresIdsByVideos = genresRelations.GroupBy(x => x.VideoId).ToList();

        foreach (var relationGroup in relationsWithGenresIdsByVideos)
        {
            var video = items.Find(x => x.Id == relationGroup.Key);

            foreach (var relation in relationGroup)
            {
                video.AddGenre(relation.GenreId);
            }
        }
    }

    private async Task AddCastMembersToVideos(List<Video> items, List<Guid> videoIds, CancellationToken cancellationToken)
    {
        var castMembersRelations  = await this.VideosCastMembers
            .Where(relation => videoIds
                .Contains(relation.VideoId))
            .ToListAsync(cancellationToken);

        var relationsWithCastMembersByVideoId = castMembersRelations.GroupBy(x => x.VideoId).ToList();

        foreach (var relationGroup in relationsWithCastMembersByVideoId)
        {
            var video = items.Find(x => x.Id == relationGroup.Key);

            foreach (var relation in relationGroup)
            {
                video.AddCastMember(relation.CastMemberId);
            }
        }
    }

    private IQueryable<Video> AddOrderToQuery(IQueryable<Video> query, string orderBy, SearchOrder order)
        => (orderBy.ToLower(), order) switch
        {
            ("title", SearchOrder.Asc) => query.OrderBy(x => x.Title).ThenBy(x=>x.Id),
            ("title", SearchOrder.Desc) => query.OrderByDescending(x => x.Title).ThenByDescending(x=>x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Title).ThenBy(x=>x.Id),
        };
}