// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;

using Common;
using Domain.Repository;
using Interfaces;
using MediatR;

public class GetCastMember(ICastMemberRepository repository) : IGetCastMember
{
    public async Task<CastMemberModelOutput> Handle(GetCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await repository.Get(request.Id, cancellationToken);
        return (CastMemberModelOutput)castMember;
    }
}