// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.Common;

using Domain.Entity;
using Domain.Enum;

public class CastMemberModelOutput(Guid id, string name, CastMemberType type, DateTime createdAt)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public CastMemberType Type { get; set; } = type;

    public DateTime CreatedAt { get; set; } = createdAt;

    public static explicit operator CastMemberModelOutput(CastMember castMember) =>
        new(castMember.Id, castMember.Name, castMember.Type, castMember.CreatedAt);
}