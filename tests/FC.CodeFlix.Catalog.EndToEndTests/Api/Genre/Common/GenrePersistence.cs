// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.Genre.Common;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Infra.Data.EF;
using Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

public class GenrePersistence(CodeflixCatalogDbContext dbContext)
{
    private readonly CodeflixCatalogDbContext dbContext = dbContext;

    public async Task InsertList(List<Genre> genres)
    {
        await this.dbContext.Genres.AddRangeAsync(genres);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task InsertGenresCategoriesRelationsList(List<GenresCategories> genresCategories)
    {
        await this.dbContext.GenresCategories.AddRangeAsync(genresCategories);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task<Genre?> GetById(Guid genreId) =>
        await this.dbContext.Genres.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == genreId);

    public async Task<List<GenresCategories>> GetGenresCategoriesRelationsByGenreId(Guid targetGenreId)
        => await this.dbContext.GenresCategories
            .AsNoTracking()
            .Where(x=>x.GenreId == targetGenreId)
            .ToListAsync();
}