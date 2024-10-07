// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.ApiModels.Response;

public class ApiResponse<TData>
{
    public TData Data { get; private set; }





    public ApiResponse(TData data) => this.Data = data;
}