using Microsoft.Extensions.Logging;

namespace TodoApp.Application.Common.Models;

public class ErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string[] Errors { get; set; } = Array.Empty<string>();

    private readonly ILogger<ErrorResponse>? _logger;

    public ErrorResponse() { }

    public ErrorResponse(int status, string message, IEnumerable<string> errors, ILogger<ErrorResponse>? logger = null)
    {
        Status = status;
        Message = message;
        Errors = errors.ToArray();
        _logger = logger;
        LogError();
    }

    public ErrorResponse(int status, string message, string error, ILogger<ErrorResponse>? logger = null)
    {
        Status = status;
        Message = message;
        Errors = new[] { error };
        _logger = logger;
        LogError();
    }

    public ErrorResponse(int status, string message, ILogger<ErrorResponse>? logger = null)
    {
        Status = status;
        Message = message;
        _logger = logger;
        LogError();
    }

    private void LogError()
    {
        if (_logger != null)
        {
            _logger.LogError("ErrorResponse created: Status={Status}, Message={Message}, Errors={Errors}",
                Status, Message, string.Join(", ", Errors));
        }
    }
}