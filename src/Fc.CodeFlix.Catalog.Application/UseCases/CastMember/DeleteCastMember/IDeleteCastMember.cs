// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

using Category.Common;
using MediatR;

public interface IDeleteCastMember : IRequestHandler<DeleteCastMemberInput, Unit>
{
    
}