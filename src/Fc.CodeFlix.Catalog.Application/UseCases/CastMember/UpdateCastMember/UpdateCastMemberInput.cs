// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

using Common;
using Domain.Enum;
using MediatR;

public class UpdateCastMemberInput(Guid id, string name, CastMemberType type) : IRequest<CastMemberModelOutput>
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public CastMemberType Type { get; set; } = type;
}