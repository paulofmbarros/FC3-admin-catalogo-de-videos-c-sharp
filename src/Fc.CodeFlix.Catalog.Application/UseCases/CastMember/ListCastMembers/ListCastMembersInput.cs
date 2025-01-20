// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers;

using Application.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;

public class ListCastMembersInput(int page, int perPage, string search, string sort, SearchOrder direction)
    : PaginatedListInput(page, perPage, search, sort, direction), IRequest<ListCastMembersOutput>;