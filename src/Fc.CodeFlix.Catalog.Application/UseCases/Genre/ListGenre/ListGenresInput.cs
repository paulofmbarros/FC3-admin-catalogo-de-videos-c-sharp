// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.UseCases.Genre.ListGenre;

using Application.Common;
using Domain.SeedWork.SearchableRepository;
using MediatR;

public class ListGenresInput(
    int page = 1,
    int perPage = 15,
    string search = "",
    string sort = "",
    SearchOrder direction = SearchOrder.Asc)
    : PaginatedListInput(page, perPage, search, sort, direction), IRequest<ListGenresOutput>;