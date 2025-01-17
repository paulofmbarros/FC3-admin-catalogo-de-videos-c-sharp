// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;

using Common;
using Domain.Entity;
using Domain.Repository;
using Interfaces;

public class CreateCastMember(ICastMemberRepository castMemberRepository, IUnitOfWork unitOfWork)
    : ICreateCastMember
{
    public async Task<CastMemberModelOutput> Handle(CreateCastMemberInput request, CancellationToken cancellationToken)
    {
        var castMember = new CastMember(request.Name, request.Type);
        await castMemberRepository.Insert(castMember, cancellationToken);

        await unitOfWork.Commit(cancellationToken);

        return (CastMemberModelOutput)castMember;
    }
}