// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;

using Application.Common;
using Common;
using Domain.Entity;
using Domain.SeedWork.SearchableRepository;

public class ListCastMembersOutput(int page, int perPage, int total, IReadOnlyList<CastMemberModelOutput> items)
    : PaginatedListOutput<CastMemberModelOutput>(page, perPage, total, items)
{
    public static ListCastMembersOutput FromSearchOutput(SearchOutput<CastMember> searchOutput) =>
        new(searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items.ToList().Select(x => (CastMemberModelOutput)x).ToList());
}