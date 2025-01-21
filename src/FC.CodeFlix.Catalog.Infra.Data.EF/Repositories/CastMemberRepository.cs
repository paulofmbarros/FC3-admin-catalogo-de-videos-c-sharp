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

    public Task Update(CastMember aggregate, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<SearchOutput<CastMember>> Search(SearchInput searchInput, CancellationToken cancellationToken) => throw new NotImplementedException();
}