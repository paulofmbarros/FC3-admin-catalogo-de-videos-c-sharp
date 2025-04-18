﻿// --------------------------------------------------------------------------------------------------------------------
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

public class CastMemberRepository(CodeflixCatalogDbContext dbContext) : ICastMemberRepository
{
    private DbSet<CastMember> CastMembers => dbContext.Set<CastMember>();
    public async Task Insert(CastMember video, CancellationToken cancellationToken) =>
        await this.CastMembers.AddAsync(video, cancellationToken);

    public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
    {
        var castMember = await this.CastMembers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(castMember, $"CastMember '{id}' not found.");

        return castMember!;
    }

    public Task Delete(CastMember video, CancellationToken cancellationToken)  =>
        Task.FromResult(this.CastMembers.Remove(video));

    public Task Update(CastMember video, CancellationToken cancellationToken) =>
        Task.FromResult(this.CastMembers.Update(video));

    public async Task<SearchOutput<CastMember>> Search(SearchInput searchInput, CancellationToken cancellationToken)
    {
        var query = this.CastMembers.AsQueryable();

        query = this.AddOrderToQuery(query, searchInput.OrderBy, searchInput.Order);

        if (string.IsNullOrWhiteSpace(searchInput.Search) is false)
        {
            query = query.Where(x => x.Name.Contains(searchInput.Search));
        }

        var data = await query
            .Skip(searchInput.PerPage * (searchInput.Page - 1))
            .Take(searchInput.PerPage)
            .ToListAsync(cancellationToken);

        var total = await query.CountAsync(cancellationToken);

        return new SearchOutput<CastMember>(searchInput.Page,searchInput.PerPage, total, data);
    }

    public async Task<IReadOnlyList<Guid>> GetIdsByIds(List<Guid> ids, CancellationToken cancellationToken) => await this
        .CastMembers
        .AsNoTracking()
        .Where(x => ids.Contains(x.Id))
        .Select(x => x.Id)
        .ToListAsync(cancellationToken);

    private IQueryable<CastMember> AddOrderToQuery(IQueryable<CastMember> query, string orderProperty, SearchOrder order)
        => (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name).ThenBy(x=>x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name).ThenByDescending(x=>x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt).ThenBy(x=>x.Name),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x=>x.Name),
            _ => query.OrderBy(x => x.Name).ThenBy(x=>x.Id),
        };
}