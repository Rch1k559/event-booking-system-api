using FluentValidation;
using RA.Utilities.Core.Exceptions;

namespace WebApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                // Group validation errors by property name to return a clean JSON error response: 
                // { "Title": ["Title is required", "Minimum length is 5"] }
                var errors = ex.Errors.GroupBy(g => g.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                await context.Response.WriteAsJsonAsync(new
                {
                    status = 400,
                    errors = errors
                });
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await context.Response.WriteAsJsonAsync(new {error = ex.Message});
            }
            catch (InvalidOperationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;

                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла непредвиденная ошибка");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new { error = "Внутренняя ошибка сервера." });
            }
        }
    }
}
