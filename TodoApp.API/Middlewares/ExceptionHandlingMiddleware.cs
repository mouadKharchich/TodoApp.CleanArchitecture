using System.Net;
using System.Text.Json;
using TodoApp.Application.Common.Exceptions;

namespace TodoApp.API.Middlewares;

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
            await _next(context); // continue pipeline
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        HttpStatusCode status;
        string message;

        switch (ex)
        {
            case NotFoundException notFound:
                status = HttpStatusCode.NotFound;
                message = notFound.Message;
                break;

            case RepositoryException repoEx:
                status = HttpStatusCode.InternalServerError;
                message = $"Repository error: {repoEx.Message}";
                break;

            case ArgumentException argEx:
                status = HttpStatusCode.BadRequest;
                message = argEx.Message;
                break;

            default:
                status = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        var response = new
        {
            StatusCode = (int)status,
            Message = message,
            ErrorType = ex.GetType().Name
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}