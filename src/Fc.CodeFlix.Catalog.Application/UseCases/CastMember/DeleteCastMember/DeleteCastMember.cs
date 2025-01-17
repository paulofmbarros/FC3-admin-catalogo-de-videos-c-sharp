// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

using Domain.Repository;
using Interfaces;
using MediatR;

public class DeleteCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork) : IDeleteCastMember
{
    public async Task<Unit> Handle(DeleteCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = await repository.Get(request.Id, cancellationToken);
        await repository.Delete(castMember, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return Unit.Value;
    }
}