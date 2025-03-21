// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Models;

using Fc.CodeFlix.Catalog.Domain.Entity;

public class VideosCategories
{
    public Category? Category { get; set; }
    public Guid CategoryId { get; set; }

    public Video? Video { get; set; }
    public Guid VideoId { get; set; }

    public VideosCategories(Guid categoryId, Guid videoId)
    {
        this.CategoryId = categoryId;
        this.VideoId = videoId;
    }
}