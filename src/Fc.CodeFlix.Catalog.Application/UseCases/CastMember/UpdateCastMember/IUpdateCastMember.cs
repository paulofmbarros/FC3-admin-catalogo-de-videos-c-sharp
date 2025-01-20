// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

using Common;
using MediatR;

public interface IUpdateCastMember : IRequestHandler<UpdateCastMemberInput, CastMemberModelOutput>
{
    
}