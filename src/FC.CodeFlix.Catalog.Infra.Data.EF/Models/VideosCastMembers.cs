// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Data.EF.Models;

using Fc.CodeFlix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VideosCastMembers
{
    public Video? Video { get; set; }
    public Guid VideoId { get; set; }

    public CastMember? CastMember { get; set; }
    public Guid CastMemberId { get; set; }

    public VideosCastMembers(Guid castMemberId, Guid videoId)
    {
        this.VideoId = videoId;
        this.CastMemberId = castMemberId;
    }

}