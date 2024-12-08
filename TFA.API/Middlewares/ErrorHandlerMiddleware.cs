using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;

namespace TFA.API.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate next;

    public ErrorHandlerMiddleware(RequestDelegate requestDelegate)
    {
        this.next = requestDelegate;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        ILogger<ErrorHandlerMiddleware> logger,
        ProblemDetailsFactory problemDetailsFactory)
    {
        try
        {
            await next.Invoke(httpContext);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                IntentionManagerException => StatusCodes.Status403Forbidden,
                DomainException domainException => domainException.StatusCode,
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            switch (ex)
            {
                case DomainException domainException:
                    logger.LogError(
                        domainException,
                        "Domain exception");
                    break;
                default:
                    logger.LogError(
                        ex,
                        "Unhandled exception");
                    break;
            }
            var problemDetails = ex switch
            {
                IntentionManagerException => problemDetailsFactory.CreateProblemDetails(httpContext, statusCode, "Error authorization"),
                DomainException domainException => problemDetailsFactory.CreateProblemDetails(httpContext, statusCode, detail: ex.Message),
                ValidationException => problemDetailsFactory.CreateValidationProblemDetails(httpContext, new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary(), statusCode, title: "Invalid request"),
                _ => problemDetailsFactory.CreateProblemDetails(httpContext, statusCode, "Some error", detail: ex.Message),
            };
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
