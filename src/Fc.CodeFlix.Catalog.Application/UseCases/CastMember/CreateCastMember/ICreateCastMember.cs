// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;

using Common;
using MediatR;

public interface ICreateCastMember : IRequestHandler<CreateCastMemberInput, CastMemberModelOutput>
{
    
}