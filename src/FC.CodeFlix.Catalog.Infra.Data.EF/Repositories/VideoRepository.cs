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

public class VideoRepository : IVideoRepository
{
    private readonly CodeflixCatalogDbContext context;
    private DbSet<Video> Videos => context.Videos;

    public VideoRepository(CodeflixCatalogDbContext context) => this.context = context;

    public async Task Insert(Video aggregate, CancellationToken cancellationToken) => await this.Videos.AddAsync(aggregate, cancellationToken);

    public Task<Video> Get(Guid id, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task Delete(Video aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task Update(Video aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<SearchOutput<Video>> Search(SearchInput searchInput, CancellationToken cancellationToken) => throw new NotImplementedException();
}