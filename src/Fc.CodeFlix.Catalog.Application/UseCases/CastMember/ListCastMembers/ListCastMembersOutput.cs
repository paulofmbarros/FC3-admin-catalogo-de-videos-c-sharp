// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;

using Application.Common;
using Common;
using Domain.Entity;

public class ListCastMembersOutput(int page, int perPage, int total, IReadOnlyList<CastMemberModelOutput> items)
    : PaginatedListOutput<CastMemberModelOutput>(page, perPage, total, items);