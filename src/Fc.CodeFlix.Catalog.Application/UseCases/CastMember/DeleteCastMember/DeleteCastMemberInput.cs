// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

using MediatR;

public class DeleteCastMemberInput(Guid id) : IRequest<Unit>
{
    public Guid Id { get; private set; } = id;
}