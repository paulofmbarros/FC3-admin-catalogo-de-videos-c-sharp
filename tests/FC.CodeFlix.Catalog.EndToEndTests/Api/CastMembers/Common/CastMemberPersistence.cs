// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CastMembers.Common;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

public class CastMemberPersistence(CodeflixCatalogDbContext dbContext)
{
    public async Task<CastMember?> GetById(Guid id)
        => await dbContext.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task InsertList(List<CastMember> exampleCastMembers)
    {
        await dbContext.CastMembers.AddRangeAsync(exampleCastMembers);
        await dbContext.SaveChangesAsync();
    }
}