using FinanceTracker.BusinessLogic.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Middleware;

// Converts known ApiExceptions into the matching ProblemDetails status code.
// Anything else (an unexpected exception) falls through to a generic 500
public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception is ApiException apiException ? apiException.StatusCode : StatusCodes.Status500InternalServerError;
        var title = exception is ApiException ? exception.Message : "An unexpected error occurred.";

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
