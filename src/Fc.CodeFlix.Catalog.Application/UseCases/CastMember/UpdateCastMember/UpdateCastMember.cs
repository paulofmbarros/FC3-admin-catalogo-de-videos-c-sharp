// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

using Common;
using Domain.Repository;
using Interfaces;

public class UpdateCastMember(IUnitOfWork unitOfWork, ICastMemberRepository repository) : IUpdateCastMember
{
    public async Task<CastMemberModelOutput> Handle(UpdateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await repository.Get(request.Id, cancellationToken);
        castMember.Update(request.Name, request.Type);
        await repository.Update(castMember, cancellationToken);
        await unitOfWork.Commit(cancellationToken);

        return (CastMemberModelOutput)castMember;
    }
}