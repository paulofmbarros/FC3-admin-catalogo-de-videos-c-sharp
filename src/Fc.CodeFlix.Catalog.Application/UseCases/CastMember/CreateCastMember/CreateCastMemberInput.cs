// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;

using Common;
using Domain.Enum;
using MediatR;

public class CreateCastMemberInput(string name, CastMemberType type) : IRequest<CastMemberModelOutput>
{
    public string Name { get; set; } = name;

    public CastMemberType Type { get; set; } = type;
}