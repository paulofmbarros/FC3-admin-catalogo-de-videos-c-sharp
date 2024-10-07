// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.ApiModels.Response;

public class ApiResponseListMeta(int currentPage, int perPage, int total)
{
    public int CurrentPage { get; set; } = currentPage;
    public int PerPage { get; set; } = perPage;
    public int Total { get; set; } = total;
}