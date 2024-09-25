// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Filters;

using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment environment;

    public ApiGlobalExceptionFilter(IHostEnvironment environment) => this.environment = environment;

    public void OnException(ExceptionContext context)
    {
        var problemDetails = new ProblemDetails();
        var exception = context.Exception;

        if(this.environment.IsDevelopment())
        {
            problemDetails.Extensions.Add("StackTrace", exception.StackTrace);
        }

        if (exception is EntityValidationException validationException)
        {
            problemDetails.Title = "One or more validation errors occurred.";
            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
            problemDetails.Type = "UnprocessableEntity";
            problemDetails.Detail = validationException.Message;
        }
        else
        {
            problemDetails.Title = "An unexpected error occurred";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Type = "Unexpected";
            problemDetails.Detail = exception.Message;
        }

        context.HttpContext.Response.StatusCode = problemDetails.Status!.Value;
        context.Result = new ObjectResult(problemDetails);
        context.ExceptionHandled = true;


    }
}