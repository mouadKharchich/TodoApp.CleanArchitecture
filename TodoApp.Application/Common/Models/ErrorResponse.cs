
namespace TodoApp.Application.Common.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    
    public ErrorResponse() { }

    public ErrorResponse(int status, string message, string errorType)
    {
        StatusCode = status;
        Message = message;
        ErrorType = errorType;
    }
    
}