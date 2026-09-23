using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RA.Utilities.Core.Exceptions;

namespace WebApi.ExceptionHandlers
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is NotFoundException)
            {
                var problemDetails = new ProblemDetails
                {
                    Detail = exception.Message,
                    Title = "Resource not found",
                    Status = StatusCodes.Status404NotFound,
                    Type = exception.GetType().Name
                };

                httpContext.Response.StatusCode = problemDetails.Status.Value;

                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

                return true;
            }

            return false;
        }
    }
}
