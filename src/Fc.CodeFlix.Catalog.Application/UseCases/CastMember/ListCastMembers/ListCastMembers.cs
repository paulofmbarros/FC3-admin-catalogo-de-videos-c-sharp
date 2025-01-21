// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;

using Common;
using Domain.Repository;
using Domain.SeedWork.SearchableRepository;

public class ListCastMembers(ICastMemberRepository repository) : IListCastMember
{
    public async Task<ListCastMembersOutput> Handle(ListCastMembersInput request, CancellationToken cancellationToken)
    {
       var searchOutput = await repository.Search(request.ToSearchInput(), cancellationToken);

        return ListCastMembersOutput.FromSearchOutput(searchOutput);

    }
}