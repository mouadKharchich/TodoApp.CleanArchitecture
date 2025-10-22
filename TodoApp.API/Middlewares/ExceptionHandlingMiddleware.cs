using System.Net;
using System.Text.Json;
using TodoApp.Application.Common.Exceptions;
using TodoApp.Application.Common.Models;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

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
            await _next(context);
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
            case ValidationException validationException:
                status = HttpStatusCode.BadRequest;
                message = validationException.Message;
                break;
            
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

        var errorResponse = new ErrorResponse
        {
            StatusCode = (int)status,
            Message = message,
            ErrorType = ex.GetType().Name
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}