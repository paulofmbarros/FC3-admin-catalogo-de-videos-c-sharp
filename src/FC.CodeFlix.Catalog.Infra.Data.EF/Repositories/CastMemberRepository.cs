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

public class CastMemberRepository(CodeflixCatalogDbContext dbContext) : ICastMemberRepository
{
    private DbSet<CastMember> CastMembers => dbContext.Set<CastMember>();
    public async Task Insert(CastMember genre, CancellationToken cancellationToken) =>
        await this.CastMembers.AddAsync(genre, cancellationToken);

    public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
    {
        var castMember = await this.CastMembers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(castMember, $"CastMember '{id}' not found.");

        return castMember!;
    }

    public Task Delete(CastMember aggregate, CancellationToken cancellationToken)  =>
        Task.FromResult(this.CastMembers.Remove(aggregate));

    public Task Update(CastMember aggregate, CancellationToken cancellationToken) =>
        Task.FromResult(this.CastMembers.Update(aggregate));

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

    private IQueryable<CastMember> AddOrderToQuery(IQueryable<CastMember> query, string orderProperty, SearchOrder order)
        => (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name).ThenBy(x=>x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name).ThenByDescending(x=>x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name).ThenBy(x=>x.Id),
        };
}