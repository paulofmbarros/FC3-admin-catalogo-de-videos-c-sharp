// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.EndToEndTests.Base;

using System.Text;
using System.Text.Json;
using Extensions.String;
using Fc.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToSnakeCase();
}


public class ApiClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions defaultJsonSerializerOptions;

    public ApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.defaultJsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string route, object payload) where TOutput : class
    {
        var response = await this.httpClient.PostAsync(route,
            new StringContent(JsonSerializer.Serialize(payload, this.defaultJsonSerializerOptions), Encoding.UTF8, "application/json"));
        var output = await GetOutput<TOutput>(response);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string route, object? queryStringParameters = null) where TOutput : class
    {
        var url = PrepareGetQueryString(route,queryStringParameters);
        var response = await this.httpClient.GetAsync(url);
        var output = await GetOutput<TOutput>(response);

        return (response, output);
    }

    private string PrepareGetQueryString(string route, object? queryStringParameters)
    {
        if (queryStringParameters is null)
        {
            return route;
        }

       var parametersJson = JsonSerializer.Serialize(queryStringParameters, this.defaultJsonSerializerOptions);
       var parametersDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(parametersJson);
       return QueryHelpers.AddQueryString(route, parametersDictionary!);

    }

    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string route) where TOutput : class
    {
        var response = await this.httpClient.DeleteAsync(route);
        var output = await GetOutput<TOutput>(response);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(string route, object payload) where TOutput : class
    {
        var response = await this.httpClient.PutAsync(route,
            new StringContent(JsonSerializer.Serialize(payload, this.defaultJsonSerializerOptions), Encoding.UTF8, "application/json"));

        var output = await GetOutput<TOutput>(response);

        return (response, output);
    }


    private  async Task<TOutput> GetOutput<TOutput>(HttpResponseMessage response) where TOutput : class
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        TOutput? output = null;

        if (!string.IsNullOrWhiteSpace(responseAsString))
        {
            output = JsonSerializer.Deserialize<TOutput>
            (responseAsString, this.defaultJsonSerializerOptions
            );
        }

        return output;
    }


}