using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ScoutNet.WebAPI.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, errors) = MapException(exception);

        if ((int)statusCode >= 500)
        {
            logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new
        {
            title,
            status = (int)statusCode,
            detail = exception.Message,
            errors,
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private static (HttpStatusCode StatusCode, string Title, object? Errors) MapException(Exception exception) =>
        exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                "Validation failed",
                validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray())),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", null),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Not found", null),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation", null),
            _ => (HttpStatusCode.InternalServerError, "Internal server error", null),
        };
}
