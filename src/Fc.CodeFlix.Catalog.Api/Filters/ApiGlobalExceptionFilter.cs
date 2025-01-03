// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Api.Filters;

using Application.Exceptions;
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

        if (exception is EntityValidationException)
        {
            problemDetails.Title = "One or more validation errors occurred.";
            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
            problemDetails.Type = "UnprocessableEntity";
            problemDetails.Detail = exception.Message;
        }
        else if (exception is NotFoundException)
        {
            problemDetails.Title = "Not Found";
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Type = "NotFound";
            problemDetails.Detail = exception.Message;
        }
        else if (exception is RelatedAggregateException)
        {
            problemDetails.Title = "Invalid Related Aggregate";
            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
            problemDetails.Type = "RelatedAggregate";
            problemDetails.Detail = exception.Message;
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