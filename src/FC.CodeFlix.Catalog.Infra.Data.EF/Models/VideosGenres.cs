// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Models;

using Fc.CodeFlix.Catalog.Domain.Entity;

public class VideosGenres
{
    public Video? Video { get; set; }
    public Guid VideoId { get; set; }

    public Genre? Genre { get; set; }
    public Guid GenreId { get; set; }

    public VideosGenres(Guid genreId, Guid videoId)
    {
        this.VideoId = videoId;
        this.GenreId = genreId;
    }
}