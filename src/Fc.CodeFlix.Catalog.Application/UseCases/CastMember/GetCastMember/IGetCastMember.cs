// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;

using Common;
using MediatR;

public interface IGetCastMember : IRequestHandler<GetCastMemberInput, CastMemberModelOutput>
{
    
}